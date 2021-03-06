﻿using Phoenix;
using Phoenix.Communication.Packets;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project_E.Lib
{
    public class Abilities
    {
        public Abilities()
        {
            Journal.Clear();
        }
        private DateTime LastLeap = DateTime.Now;
        public void probo(DateTime HiddenTime, Action hidoff)
        {
            UOCharacter target = new UOCharacter(Aliases.GetObject("laststatus"));
            bool first = true;
            while (World.Player.Hidden)
            {

                UO.Wait(100);
                if (!target.Serial.Equals(Aliases.GetObject("laststatus")))
                    target = new UOCharacter(Aliases.GetObject("laststatus"));
                if (DateTime.Now - HiddenTime < TimeSpan.FromSeconds(3)) continue;
                if (first)
                {
                    UO.PrintError("Muzes Bodat!");
                    first = false;
                }
                if (target.Distance < 2)
                {
                    Journal.Clear();
                    target.WaitTarget();
                    UO.Say(".usehand");
                    Journal.WaitForText(true, 500, "Utok se nepovedl.", "Cil prilis daleko.", "Nevidis na cil.", "Nedosahnes na cil.");
                }

            }
            hidoff();
            UO.Say(",hid");

        }

        public void bomba(WeaponsSet.Weapons weapons)
        {
            UOCharacter cil = new UOCharacter(Aliases.GetObject("laststatus"));
            if (cil.Distance > 5)
            {
                UO.PrintError("Moc daleko");
                return;
            }
            cil.WaitTarget();
            UO.Say(".throwexplosion");
            UO.Wait(100);
            weapons.ActualWeapon.Equip();
        }

        public void kudla(WeaponsSet.Weapons weapons)
        {
            UOCharacter cil = new UOCharacter(Aliases.GetObject("laststatus"));
            if (cil.Distance > 6)
            {
                UO.PrintError("Moc daleko");
                return;
            }
            UO.Say(".throw");
            weapons.ActualWeapon.Equip();
            if (Journal.WaitForText(true, 1000, "Nemas zadny cil.", "Nevidis na cil"))
            {
                UO.PrintInformation("HAZ!!");
                return;
            }
            UO.Wait(1000);
            UO.PrintInformation("3");
            UO.Wait(1000);
            UO.PrintInformation("2");
            UO.Wait(1000);
            UO.PrintInformation("1");
            UO.Wait(1000);
            UO.PrintInformation("HAZ!!");
            World.Player.Print("Hazej!!!");

        }


        public void Leap()
        {
            if (DateTime.Now - LastLeap > TimeSpan.FromMilliseconds(4400))
            {
                UOCharacter ch = new UOCharacter(Aliases.LastAttack);
                if (ch.Distance < 10 && World.Player.Warmode)
                {
                    if (!World.Player.Warmode)
                    {
                        UO.Warmode(true);
                    }
                    UO.Attack(ch);
                    UO.Say(".leap");
                    LastLeap = DateTime.Now;
                    while (DateTime.Now - LastLeap < TimeSpan.FromMilliseconds(4400))
                    {
                        UO.Wait(100);
                    }
                    World.Player.Print("===== LEAP =====");
                }
                else
                    UO.PrintError("Moc Daleko");

            }
            else
                UO.PrintError("Jeste nemuzes pouzit Leap");
        }


        #region Voodoo

        // TODO autoVOooDOO dung and home

        enum VoodooState
        {
            Fail,
            Success,
            Wait,
            Redo,
            NoTarget
        }
        Dictionary<string, UOItem> boostBottles = new Dictionary<string, UOItem>()
        {
            { "str", null},
            { "dex", null},
            { "int", null},
            { "def", null}

        };
        List<Graphic> HeadGraphics = new List<Graphic>() { 0x1DAE, 0x1DA0, 0x1CE9, 0x1CE1 };

        private VoodooState done;


        public void Sacrafire()//Action bandage)
        {
            if (World.Player.Mana == World.Player.MaxMana)
            {
                UO.PrintInformation("Mas plnou manu!!");
                if (World.Player.Hits < World.Player.MaxHits) UO.Say(",bandage");
                return;
            }
            if (World.Player.Hits > 80)
            {
                if (World.Player.Hits < World.Player.MaxHits)
                {
                    UO.Say(",bandage");
                    UO.Say(".voodooobet");
                }
                else
                {
                    UO.Say(".voodooobet");
                    UO.Wait(100);
                    UO.Say(",bandage");
                }
                UO.Wait(100);
            }
            else UO.PrintWarning("malo HP!!");
        }



        public void boost(string type)
        {
            boostBottles["str"] = new UOItem(World.Player.Backpack.AllItems.FindType(0x0F0E, 0x0835));
            boostBottles["dex"] = new UOItem(World.Player.Backpack.AllItems.FindType(0x0F0E, 0x0006));
            boostBottles["int"] = new UOItem(World.Player.Backpack.AllItems.FindType(0x0F0E, 0x06C2));
            boostBottles["def"] = new UOItem(World.Player.Backpack.AllItems.FindType(0x0F0E, 0x0999));
            List<UOItem> Heads = new List<UOItem>();
            UOItem head = new UOItem(0x01);

            while(head.Serial!=0xFFFFFFFF)
            {
                UO.Wait(200);
                head = new UOItem(UIManager.TargetObject());

                if (head.Serial != 0xFFFFFFFF)
                {
                    Heads.Add(head);

                }
            }



            try
            {
                Core.RegisterServerMessageCallback(0x1C, onVoodoo);
                Serial cont;
                ushort X, Y;
                foreach (var it in Heads.Where(x=>HeadGraphics.Any(y=>x.Graphic==y)).ToList())
                {
                    cont = it.Container;
                    X = it.X;
                    Y = it.Y;
                    it.Move(1, World.Player.Backpack);
                    UO.Wait(100);

                    done = VoodooState.Fail;
                    if (boostBottles[type] == null || boostBottles[type].Serial == 0xFFFFFFFF || boostBottles[type].Serial == Serial.Invalid)
                    {
                        UO.PrintError("Nemas {0} lahev.", type);
                        return;
                    }
                    while (done != VoodooState.Wait)
                    {
                        UO.Wait(200);
                        if (done == VoodooState.NoTarget)
                        {
                            UO.Wait(4000);
                            break;
                        }
                        boostBottles[type].WaitTarget();
                        it.Use();
                        UO.Wait(200);

                    }
                    while (done != VoodooState.Success)
                    {
                        UO.Wait(500);
                        if (done == VoodooState.NoTarget) break;
                    }
                    it.Move(1, cont, X, Y);
                    UO.Wait(100);

                }
            }
            catch (Exception ex) { UO.PrintError(ex.InnerException.Message); }
            finally
            {

                Core.UnregisterServerMessageCallback(0x1C, onVoodoo);
            }

        }


        public void selfboost(string type)
        {
            boostBottles["str"] = new UOItem(World.Player.Backpack.AllItems.FindType(0x0F0E, 0x0835));
            boostBottles["dex"] = new UOItem(World.Player.Backpack.AllItems.FindType(0x0F0E, 0x0006));
            boostBottles["int"] = new UOItem(World.Player.Backpack.AllItems.FindType(0x0F0E, 0x06C2));
            boostBottles["def"] = new UOItem(World.Player.Backpack.AllItems.FindType(0x0F0E, 0x0999));

            UOItem head = null;

            foreach (var it in World.Player.Backpack.AllItems.Where(x => HeadGraphics.Any(y=>x.Graphic==y)).ToList())
            {
                it.Click();
                UO.Wait(100);
                if (it.Name == World.Player.Name) head = new UOItem(it);
            }
            if (head == null)
            {
                UO.Print("Nemas svou hlavu");
                if (type == "int")
                {
                    UO.Cast(StandardSpell.Cunning, World.Player);
                    UO.Wait(2200);
                    UO.Cast(StandardSpell.Protection, World.Player);
                }
                return;
            }
            try
            {
                Core.RegisterServerMessageCallback(0x1C, onVoodoo);
                done = VoodooState.Fail;
                if (boostBottles[type] == null || boostBottles[type].Serial == 0xFFFFFFFF || boostBottles[type].Serial == Serial.Invalid)
                {
                    UO.PrintError("Nemas {0} lahev.", type);
                    return;
                }
                while (done != VoodooState.Wait)
                {
                    boostBottles[type].WaitTarget();
                    head.Use();
                    UO.Wait(200);
                }
                while (done != VoodooState.Success) UO.Wait(400);
                UO.Wait(100);

            }
            catch (Exception ex) { UO.PrintError(ex.InnerException.Message); }
            finally
            {

                Core.UnregisterServerMessageCallback(0x1C, onVoodoo);
            }



        }
        



        CallbackResult onVoodoo(byte[] data, CallbackResult prev)//0x1C
        {
            AsciiSpeech ass = new AsciiSpeech(data);
            if (ass.Text.Contains("Nepovedlo se")) done = VoodooState.Fail;
            if (ass.Text.Contains("Cil podlehl ")) done = VoodooState.Success;
            if (ass.Text.Contains("Jeste nelze pouzit.")) done = VoodooState.Redo;
            if (ass.Text.Contains("prokleti voodoo seslano uspesne")) done = VoodooState.Wait;
            if (ass.Text.Contains("prokleti nenalezlo cil.")) done = VoodooState.NoTarget;
            return CallbackResult.Normal;
        }
        #endregion
    }
}
