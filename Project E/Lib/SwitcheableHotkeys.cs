using Phoenix;
using Phoenix.Gui.Pages;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Project_E.Lib
{
    [Serializable]
    public class SwitchabeHotkeys
    {

        public List<string> swHotkeys { get; set; }
        private bool on = false;

        Phoenix.Runtime.Hotkeys h = Phoenix.Runtime.RuntimeCore.Hotkeys;
        public SwitchabeHotkeys()
        {
            swHotkeys = new List<string>();
        }


        public void swHotk()
        {
            foreach (string s in swHotkeys)
            {
                string[] tmp = s.Split(';');
                if (h.Contains(tmp[0])) h.Remove(tmp[0]);
            }
            if (!on)
            {

                foreach (string s in swHotkeys)
                {
                    string[] tmp = s.Split(';');
                    h.Add(tmp[0], tmp[1]);
                }
                UO.PrintInformation("Hotkeys ON");
                on = true;
            }
            else
            {
                foreach (string s in swHotkeys)
                {
                    string[] tmp = s.Split(';');
                    if (h.Contains(tmp[0])) h.Remove(tmp[0]);
                }
                UO.PrintWarning("Hotkeys OFF");
                on = false;
            }
        }

        public void Add()
        {

            try
            {
                AddDialog dlg = new AddDialog();
                dlg.StartPosition = FormStartPosition.CenterScreen;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    h.Add(dlg.Shortcut, dlg.Command);
                    swHotkeys.Add(dlg.Shortcut + ";" + dlg.Command);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

        }
        public void Clear()
        {
            foreach (string s in swHotkeys)
            {
                string[] tmp = s.Split(';');
                if (h.Contains(tmp[0])) h.Remove(tmp[0]);

            }
            on = false;
            swHotkeys.Clear();
        }

        public void Add(string Code)
        {
            if (Code == "NULL") return;
            string[] tmp = Code.Split('_');
            foreach (string s in tmp)
            {
                swHotkeys.Add(s);
            }
        }
        public override string ToString()
        {
            string tmp = "";
            if (swHotkeys.Count < 1) return "NULL";
            foreach (string e in swHotkeys)
            {
                tmp += e + "_";
            }
            tmp = tmp.Remove(tmp.Length - 1);
            return tmp;
        }
    }
}
