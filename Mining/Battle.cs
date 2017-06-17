using Phoenix;
using Phoenix.Communication;
using Phoenix.WorldData;
using Project_E;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Mining
{
    internal class Battle
    {

        List<string> TopMonster = new List<string>() { "golem", "spirit" };
        Graphic[] Humanoid = { 0x0191, 0x0190 };
        Action<int, int> MoveTo;
        Action<int> MoveXfield;
        Action<int> Recall;
        UOCharacter Enemy;
        UOItem Weapon;
        Point ActualPosition;
        private bool StoodUp;

        public Battle(Action<int, int> moveTo, Action<int> moveXfield, Action<int> recall, UOCharacter Ch, UOItem weapon)
        {
            MoveTo = moveTo;
            MoveXfield = moveXfield;
            Enemy = Ch;
            Weapon = weapon;
            Recall = recall;
            ActualPosition = new Point(World.Player.X, World.Player.Y);
        }

        public void Kill()
        {
            Enemy.Click();
            UO.Wait(200);
            try
            {
                Core.RegisterServerMessageCallback(0x6E, onStoodUp);
                UO.Attack(Enemy);
                Weapon.Equip();
                if (Enemy.Distance > 1) MoveTo(Enemy.X, Enemy.Y);
                while (!Journal.Contains(true, "Ziskala jsi ", "Ziskal jsi ", "gogo"))
                {
                    if (!Enemy.Exist || Enemy.Hits < 1)
                    {
                        UO.Wait(500);
                        MoveTo(ActualPosition.X, ActualPosition.Y);
                        return;
                    }

                    if (Enemy.Distance > 1 && !StoodUp) MoveTo(Enemy.X-1, Enemy.Y);
                    if(StoodUp && !Enemy.Name.Contains("Rock "))
                    {
                        MoveXfield(6);
                        StoodUp = false;
                    }
                    if (Journal.Contains("Vysavas zivoty!"))
                    {
                        Journal.SetLineText(Journal.Find("Vysavas zivoty!"), " ");
                        Main.Instance.AH.Bandage();
                        Weapon.Equip();
                        try
                        {
                            //UO.Attack(Enemy);
                        }
                        catch { }
                   }
                    UO.Wait(100);
                }

                MoveTo(ActualPosition.X, ActualPosition.Y);
            }
            catch (Exception ex) { UO.PrintError(ex.Message); }
            finally
            {
                Core.UnregisterServerMessageCallback(0x6E, onStoodUp);

            }

        }




        CallbackResult onStoodUp(byte[] data, CallbackResult prev)
        {

            PacketReader p = new PacketReader(data);
            p.Skip(1);
            uint serial = p.ReadUInt32();
            ushort action = p.ReadUInt16();
            if ((action == 26 || action == 11) && serial == World.Player.Serial && new UOCharacter(Aliases.LastAttack).Distance < 3)
            {
                //UO.Print(SpeechFont.Normal, 0x0076, "Naprah na " + new UOCharacter(Aliases.LastAttack).Name);
                StoodUp = true;
            }

            return CallbackResult.Normal;
        }
    }
}
