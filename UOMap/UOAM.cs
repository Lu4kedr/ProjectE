using Phoenix;
using Phoenix.Communication;
using Phoenix.Communication.Packets;
using Phoenix.Runtime;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace UOMap
{
    [RuntimeObject]
    public class AssistApi : IDisposable
    {
        private const string ApiKey = "UOASSIST-TP-MSG-WND";

        public AssistApi()
        {
            alive = true;
            finished = new ManualResetEvent(false);

            RuntimeCore.UnregisteringAssembly += new UnregisteringAssemblyEventHandler(RuntimeCore_UnregisteringAssembly);
            Core.LoginComplete += new EventHandler(Core_LoginComplete);

            applications = new Dictionary<IntPtr, uint>();
            commands = new Dictionary<string, CommandInfo>();

            apiThread = new Thread(new ThreadStart(Work));
            apiThread.IsBackground = true;
            apiThread.Start();
        }
        ~AssistApi()
        {
            Dispose();
        }

        private Thread apiThread;
        private bool alive;
        private ManualResetEvent finished;

        private Dictionary<IntPtr, uint> applications;
        private Dictionary<string, CommandInfo> commands;

        private void Work()
        {
            try
            {
                Message message;
                NativeMethods.PeekMessage(out message, IntPtr.Zero, 0, 0, 0);

                WndProcDelegate wndProc = new WndProcDelegate(WndProc);

                WNDCLASS windowClass = new WNDCLASS();
                windowClass.lpszClassName = ApiKey;
                windowClass.lpfnWndProc = Marshal.GetFunctionPointerForDelegate(wndProc);

                Debug.WriteLine("Registering window", "AssistApi");
                ushort windowClassAtom = NativeMethods.RegisterClass(ref windowClass);
                if (windowClassAtom == 0 && Marshal.GetLastWin32Error() != 1410)
                    throw new Win32Exception();

                Debug.WriteLine("Creating window", "AssistApi");
                IntPtr windowHandle = NativeMethods.CreateWindowEx(0, ApiKey, ApiKey, 0, 0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                if (windowHandle == IntPtr.Zero)
                    throw new Win32Exception();

                Debug.WriteLine("Entering main loop", "AssistApi");

                while (alive)
                {
                    while (NativeMethods.PeekMessage(out message, IntPtr.Zero, 0, 0, 0))
                    {
                        if (NativeMethods.GetMessage(out message, IntPtr.Zero, 0, 0) == -1)
                            throw new Win32Exception();

                        NativeMethods.TranslateMessage(ref message);
                        NativeMethods.DispatchMessage(ref message);
                    }
                    Thread.Sleep(100);
                }

                Debug.WriteLine("Destroying window", "AssistApi");
                if (!NativeMethods.DestroyWindow(windowHandle))
                    throw new Win32Exception();

                Debug.WriteLine("Unregistering window", "AssistApi");
                if (!NativeMethods.UnregisterClass(ApiKey, IntPtr.Zero))
                    throw new Win32Exception();
            }
            catch (Exception e1)
            {
                Debug.WriteLine(e1.Message, "Error");
                Debug.WriteLine(e1.StackTrace, "Error");
            }
            finally
            {
                finished.Set();
            }
        }

        private string ReadAtomAndDelete(ushort atom)
        {
            StringBuilder result = new StringBuilder(255);

            if (NativeMethods.GlobalGetAtomName(atom, result, result.Capacity) == 0)
                throw new Win32Exception();

            NativeMethods.GlobalDeleteAtom(atom);
            if (Marshal.GetLastWin32Error() != 0)
                throw new Win32Exception();

            return result.ToString();
        }

        private void CleanDead()
        {
            foreach (IntPtr application in (from a in applications.Keys
                                            where !NativeMethods.IsWindow(a)
                                            select a).ToList())
                applications.Remove(application);

            foreach (string command in (from c in commands.Values
                                        where !NativeMethods.IsWindow(c.Handle)
                                        select c.Name).ToList())
                commands.Remove(command);
        }

        private IntPtr WndProc(IntPtr hwnd, uint uMsg, IntPtr wParam, IntPtr lParam)
        {

            CleanDead();

            switch (uMsg)
            {

                case 0x400 + 200:
                    if (applications.ContainsKey(wParam))
                    {
                        foreach (string command in (from c in commands.Values
                                                    where c.Handle == wParam
                                                    select c.Name).ToList())
                            commands.Remove(command);

                        Debug.WriteLine("Unregistered application", "AssistApi");
                        applications.Remove(wParam);
                        return new IntPtr(2);
                    }

                    Debug.WriteLine("Registered new application", "AssistApi");
                    applications.Add(wParam, (uint)lParam);

                    return new IntPtr(1);

                case 0x400 + 202:
                    return new IntPtr(World.Player.X | (World.Player.Y << 16));

                case 0x400 + 207:
                    ushort messageColor = (ushort)wParam.ToInt32();
                    bool messageSystem = ((wParam.ToInt32() >> 16) & 1) == 1;
                    string message = ReadAtomAndDelete((ushort)lParam);

                    Core.SendToClient(PacketBuilder.CharacterSpeechUnicode(messageSystem ? Serial.Invalid : (uint)World.Player.Serial, World.Player.Model, "UOAM", SpeechType.Regular, SpeechFont.Normal, messageColor, message));
                    break;

                case 0x400 + 209:
                    string commandName = ReadAtomAndDelete((ushort)lParam);

                    if (wParam == IntPtr.Zero)
                    {
                        commands.Remove(commandName);
                        Debug.WriteLine(string.Format("Unregistered command '{0}'", commandName), "AssistApi");
                        return IntPtr.Zero;
                    }

                    CommandInfo commandInfo = new CommandInfo(wParam, commandName);
                    commands.Add(commandName, commandInfo);
                    Debug.WriteLine(string.Format("Registered command '{0}' (slot {1})", commandName, commandInfo.Offset), "AssistApi");
                    return new IntPtr(0x400 + 0x400 + commandInfo.Offset);

                default:
                    if (uMsg > 0x400)
                        Debug.WriteLine(string.Format("Unknown message 0x{0:x4} w: 0x{1:x4} l: 0x{2:x4}", uMsg, wParam, lParam), "AssistApi");
                    break;

            }

            return NativeMethods.DefWindowProc(hwnd, uMsg, wParam, lParam);
        }

        #region Event handlers

        [ClientMessageHandler(0x03, Priority = CallbackPriority.Highest)]
        [ClientMessageHandler(0xad, Priority = CallbackPriority.Highest)]
        public CallbackResult OnSpeechRequest(byte[] data, CallbackResult prevResult)
        {
            string text;

            text = data[0] == 0x03 ? new AsciiSpeechRequest(data).Text : new UnicodeSpeechRequest(data).Text;

            if (text.StartsWith("-"))
            {
                string commandName = text.Substring(1);
                foreach (CommandInfo command in commands.Values)
                {
                    if (commandName.StartsWith(command.Name.ToLower()))
                    {
                        command.Invoke(commandName.Substring(command.Name.Length));
                        return CallbackResult.Eat;
                    }
                }

                UO.PrintError("Unknown assist api command");
                return CallbackResult.Eat;
            }

            return prevResult;
        }

        private void RuntimeCore_UnregisteringAssembly(object sender, Phoenix.Runtime.UnregisteringAssemblyEventArgs e)
        {
            Dispose();
        }

        private void Core_LoginComplete(object sender, EventArgs e)
        {
            foreach (IntPtr handle in applications.Keys)
            {
                Debug.WriteLine(string.Format("Sending login notification to 0x{0:x8}", handle), "AssistApi");
                NativeMethods.SendMessage(handle, 0x400 + 303, new IntPtr(World.Player.Serial), IntPtr.Zero);
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            alive = false;
            finished.WaitOne();
            finished.Close();

            GC.SuppressFinalize(this);
        }

        #endregion

        #region Delegates

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate IntPtr WndProcDelegate(IntPtr hwnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        #endregion

        #region Nested type: CommandInfo

        private class CommandInfo
        {
            public CommandInfo(IntPtr handle, string name)
            {
                Handle = handle;
                Name = name;

                Offset = offset++;
            }

            private static int offset;

            public void Invoke(string args)
            {
                ushort atom = NativeMethods.GlobalAddAtom(args);

                NativeMethods.PostMessage(Handle, (uint)(0x400 + 0x400 + Offset), new IntPtr(atom), IntPtr.Zero);
            }

            public IntPtr Handle
            {
                get;
                private set;
            }

            public int Offset
            {
                get;
                private set;
            }

            public string Name
            {
                get;
                private set;
            }
        }

        #endregion

        #region Nested type: WNDCLASS

        private struct WNDCLASS
        {
            public uint style;
            public IntPtr lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpszMenuName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpszClassName;
        }

        #endregion

        #region Native methods

        private class NativeMethods
        {

            [DllImport("kernel32", SetLastError = true)]
            public static extern uint GlobalGetAtomName(ushort nAtom, [MarshalAs(UnmanagedType.LPStr)] StringBuilder lpBuffer, int nSize);

            [DllImport("kernel32", SetLastError = true)]
            public static extern ushort GlobalAddAtom([MarshalAs(UnmanagedType.LPStr)] string lpString);

            [DllImport("kernel32", SetLastError = true)]
            public static extern ushort GlobalDeleteAtom(ushort nAtom);

            [DllImport("user32", SetLastError = true)]
            public static extern IntPtr CreateWindowEx(
                uint dwExStyle,
                [MarshalAs(UnmanagedType.LPStr)] string lpClassName,
                [MarshalAs(UnmanagedType.LPStr)] string lpWindowName,
                uint dwStyle,
                int x,
                int y,
                int nWidth,
                int nHeight,
                IntPtr hWndParent,
                IntPtr hMenu,
                IntPtr hInstance,
                IntPtr lpParam
            );

            [DllImport("user32", SetLastError = true)]
            public static extern bool DestroyWindow(IntPtr hWnd);

            [DllImport("user32")]
            public static extern IntPtr DefWindowProc(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32", SetLastError = true)]
            public static extern ushort RegisterClass(ref WNDCLASS lpWndClass);

            [DllImport("user32", SetLastError = true)]
            public static extern bool UnregisterClass([MarshalAs(UnmanagedType.LPStr)] string lpClassName, IntPtr hInstance);

            [DllImport("user32")]
            public static extern bool PeekMessage(out Message lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

            [DllImport("user32", SetLastError = true)]
            public static extern int GetMessage(out Message lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

            [DllImport("user32")]
            public static extern bool TranslateMessage(ref Message lpMsg);

            [DllImport("user32")]
            public static extern bool DispatchMessage(ref Message lpMsg);

            [DllImport("user32")]
            public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32")]
            public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32")]
            public static extern IntPtr FindWindow([MarshalAs(UnmanagedType.LPStr)] string lpClassName, [MarshalAs(UnmanagedType.LPStr)] string lpWindowName);

            [DllImport("user32")]
            public static extern bool IsWindow(IntPtr hWnd);

        }

        #endregion
    }
}
