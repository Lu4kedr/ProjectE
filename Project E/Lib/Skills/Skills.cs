using Phoenix;
using Phoenix.Communication;
using Phoenix.WorldData;
using Project_E.GUI;
using Project_E.Lib.Healing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project_E.Lib.Skills
{
    public class Skills
    {
        SettingsGUI Settings;
        public Skills(SettingsGUI SGUI)
        {
            Settings = SGUI;
        }
        #region Music

        public void Peace_Entic(StandardSkill Skill, Serial Target, ref bool MusicDone)
        {
            if (Target == null)
            {
                MusicDone = true;
                return;
            }
            UOCharacter tmp = new UOCharacter(Target);
            MusicDone = false;
            if (tmp.Distance > 18)
            {
                MusicDone = true;
                return;
            }
            tmp.WaitTarget();
            UO.UseSkill(Skill);

        }

        public void Provo(Serial Target1, Serial Target2, ref bool MusicDone)
        {
            if (Target1 == null || Target2 == null)
            {
                MusicDone = true;
                return;
            }
            UOCharacter tmp1 = new UOCharacter(Target1);
            UOCharacter tmp2 = new UOCharacter(Target2);
            MusicDone = false;
            if (tmp1.Distance > 18 || tmp2.Distance > 18)
            {
                MusicDone = true;
                return;
            }
            tmp1.WaitTarget();
            UO.Say(".provo");
            tmp2.WaitTarget();


        }




        #endregion

        #region Poisoning

        public void Poisoning()
        {
            if (Main.Instance.SGUI.Poison == default(ushort))
            {
                UO.PrintError("Nemas nastaveny poison");
                return;
            }

            UOItem zbran = World.Player.Layers[Layer.RightHand];

            UOItem pois = World.Player.Backpack.AllItems.FindType(0x0F0E, Main.Instance.SGUI.Poison);
            if (pois == null)
            {
                UO.PrintError("Nemas nastaveny typ poisonu");
                return;
            }

            zbran.WaitTarget();
            pois.Use();
        }


        #endregion

        // TODO Veterinary

        #region Hiding
        private bool getHit;
        public void Hiding(bool first)
        {
            Core.UnregisterClientMessageCallback(0x02, makeStep);
            getHit = false;
            Main.Instance.WT.HitsChanged += WT_HitsChanged;

            int step = 2;
            UO.Warmode(false);
            UO.UseSkill(StandardSkill.Hiding);
            World.Player.Print("3");
            DateTime startHid = DateTime.Now;
            while (DateTime.Now - startHid < TimeSpan.FromMilliseconds(double.Parse(Settings.HiddingDelay)))
            {
                if (getHit)
                {
                    Core.UnregisterClientMessageCallback(0x02, makeStep);
                    Main.Instance.WT.HitsChanged -= WT_HitsChanged;
                    if (first) Hiding(false);
                    return;
                }
                if (DateTime.Now - startHid > TimeSpan.FromMilliseconds(double.Parse(Settings.HiddingDelay) / 3) && step == 2)
                {
                    World.Player.Print("2");
                    step--;
                }
                if (DateTime.Now - startHid > TimeSpan.FromMilliseconds((double.Parse(Settings.HiddingDelay) / 3) * 2) && step == 1)
                {
                    World.Player.Print("1");
                    step--;

                    Core.RegisterClientMessageCallback(0x02, makeStep);
                }
                UO.Wait(10);
            }
            if (Journal.WaitForText(true, 500, "Nepovedlo se ti schovat.", "Skryti se povedlo."))
            {
                UO.Wait(200);
                if (!World.Player.Hidden)
                    Core.UnregisterClientMessageCallback(0x02, makeStep);
            }
            else Core.UnregisterClientMessageCallback(0x02, makeStep);
            Main.Instance.WT.HitsChanged -= WT_HitsChanged;

        }

        private void WT_HitsChanged(object sender, Watcher.HitsChangedArgs e)
        {
            if (!e.gain) getHit = true;
        }

        public void hidoff()
        {
            Core.UnregisterClientMessageCallback(0x02, makeStep);
        }

        CallbackResult makeStep(byte[] data, CallbackResult prev)//0x02 clientReq
        {
            PacketReader pr = new PacketReader(data);
            PacketWriter pw = new PacketWriter();
            byte cmd = pr.ReadByte();
            byte dir = pr.ReadByte();
            byte seq = pr.ReadByte();
            uint fwalkPrev = pr.ReadUInt32();
            if (Convert.ToUInt16(dir) > 7)
            {
                dir = Convert.ToByte(Convert.ToUInt16(dir) - 128);
                pw.Write(cmd);
                pw.Write(dir);
                pw.Write(seq);
                pw.Write(fwalkPrev);
                Core.SendToServer(pw.GetBytes());
                return CallbackResult.Sent;
            }

            return CallbackResult.Normal;
        }
        #endregion

        #region Magery

        Dictionary<string, int> SpellsDelays = new Dictionary<string, int> { { "Fireball", 2060 }, { "Flame", 4100 }, { "Meteor", 6750 }, { "Lightning", 3550 }, { "Bolt", 3100 }, { "frostbolt", 3000 }, { "Harm", 1500 }, { "Mind", 3450 }, { "Invis", 3300 }/*-150*/ };


        public void ccast(string spellname, Serial target, Action bandage, Action Sacrafire)
        {
            if (World.Player.Hits < World.Player.MaxHits - 7) bandage();
            if (Settings.AutoSacrafire && Settings.KlerikShaman == 1 && World.Player.Mana < World.Player.MaxMana - short.Parse(Settings.Sacrafire)) Sacrafire();
            UO.Attack(target);
            Aliases.LastAttack = target;
            if (spellname == "necrobolt" || spellname == "frostbolt")
            {
                UO.WaitTargetObject(target);
                UO.Say("." + spellname);
            }
            else
            {
                UO.Cast(spellname, target);
            }
        }

        public void ccast(string spellname, Serial target, Action bandage, Action Sacrafire, AutoHeal Aheal)
        {
            if(Aheal.Running)Aheal.Stop();
            if (World.Player.Hits < World.Player.MaxHits - 7) bandage();
            if (Settings.AutoSacrafire && Settings.KlerikShaman == 1 && World.Player.Mana < World.Player.MaxMana - short.Parse(Settings.Sacrafire)) Sacrafire();
            UO.Attack(target);
            Aliases.LastAttack = target;
            if (spellname == "necrobolt" || spellname == "frostbolt")
            {
                UO.WaitTargetObject(target);
                UO.Say("." + spellname);
            }
            else
            {
                UO.Cast(spellname, target);
            }
            Journal.WaitForText(true, SpellsDelays[spellname] + 100,"kouzlo se nezdarilo.");
            if (Aheal.Running) Aheal.Start();
        }


        public void ReactiveArmor(Serial target)
        {
            DateTime start;
            UO.Warmode(false);
            UO.Cast(StandardSpell.ReactiveArmor, target);
            if (!Journal.WaitForText(true, 2000, "Kouzlo se nezdarilo."))
            {
                start = DateTime.Now;
            }
            else return;
            while (DateTime.Now - start < TimeSpan.FromSeconds(8)) UO.Wait(100);
            UO.PrintError("Reactiv vyprsel");
        }

        public void Invis(Action bandage, Action Sacrafire)
        {
            int tmp = 8;
            UO.Warmode(false);
            if (World.Player.Hits < World.Player.MaxHits - 7) bandage();
            ccast("Invis", Aliases.Self, bandage, Sacrafire);

            if (Journal.WaitForText(true, SpellsDelays["Invis"] / 3, "Kouzlo se nezdarilo.")) return;
            World.Player.Print(3);
            if (Journal.WaitForText(true, SpellsDelays["Invis"] / 3, "Kouzlo se nezdarilo.")) return;
            World.Player.Print(2);
            if (!Journal.WaitForText(true, SpellsDelays["Invis"] / 3, "Kouzlo se nezdarilo."))
            {
                World.Player.Print(1);
                if (clearPerimeter(2))
                {
                    DateTime start = DateTime.Now;
                    UO.PrintInformation("Blokuju krok na 1 s");
                    Core.UnregisterClientMessageCallback(0x02, blockStep);
                    Core.RegisterClientMessageCallback(0x02, blockStep);
                    while ((DateTime.Now - start) < TimeSpan.FromMilliseconds(1000))
                    {
                        UO.Wait(50);
                        tmp--;
                        if (!clearPerimeter(2))
                        {
                            Core.UnregisterClientMessageCallback(0x02, blockStep);
                            return;
                        }
                        if (tmp < 0 && !World.Player.Hidden)
                        {
                            Core.UnregisterClientMessageCallback(0x02, blockStep);
                            return;
                        }
                    }
                    Core.UnregisterClientMessageCallback(0x02, blockStep);
                    if (World.Player.Hits < World.Player.MaxHits - 7) bandage();
                }
            }
        }
        private bool clearPerimeter(int perimeterSize)
        {
            foreach (UOCharacter ch in World.Characters.Where(cha => cha.Distance < perimeterSize && cha.Notoriety > Notoriety.Neutral))
            {
                return false;
            }
            return true;
        }

        public void HarmSelf(bool attacklast)
        {
            UO.Warmode(false);
            UO.Cast(StandardSpell.Harm, World.Player.Serial);
            UO.Wait(1600);
            if (attacklast)
            {
                UO.Warmode(true);
                UO.Attack(Aliases.GetObject("laststatus").IsValid ? Aliases.GetObject("laststatus") : 0x00);
            }
        }


        CallbackResult blockStep(byte[] data, CallbackResult prev)//0x02 clientReq
        {
            UO.Say(".resync");
            return CallbackResult.Eat;
        }





        public void AutoCast(string spellname, Action bandage, Action Sacrafire, ref bool autocast)
        {
            if (autocast)
            {
                autocast = false;
                UO.PrintError("Autocast OFF");
            }
            else
            {
                autocast = true;
                UO.PrintError("Autocast ON");
                DateTime start;
                var target = Aliases.LastAttack;
                while (autocast)
                {
                    ccast(spellname, target,bandage,Sacrafire);
                    start = DateTime.Now;
                    while ((DateTime.Now - start) < TimeSpan.FromMilliseconds(SpellsDelays[spellname]))
                    {
                        if (!autocast) return;
                        UO.Wait(50);
                    }

                }

            }




        }

        // TODO Autocast

        public void AutoCast_Charged(string spellname,Action bandage, Action Sacrafire, ref bool autocast)
        {
            int counter = 0;
            if (autocast)
            {
                autocast = false;
                UO.PrintError("Autocast OFF");
            }
            else
            {
                autocast = true;
                UO.PrintError("Autocast ON");
                DateTime start;
                var target = Aliases.LastAttack;
                while (autocast)
                {
                    if (counter % 5 == 0)
                        ChargeSpell(spellname, ref autocast);
                    ccast(spellname, target, bandage, Sacrafire);
                    start = DateTime.Now;
                    while ((DateTime.Now - start) < TimeSpan.FromMilliseconds(SpellsDelays[spellname]))
                    {
                        if (!autocast) return;
                        UO.Wait(50);
                    }
                    counter++;

                }

            }

        }

        private void ChargeSpell(string spellname, ref bool autocast)
        {
            DateTime start;
            if (new[] { "Fireball", "Flame", "Meteor" }.Contains(spellname))
            {
                for (int i = 0; i < 3; i++)
                {
                    if (!autocast)
                    {
                        UO.Print("Autocast OFF");
                        return;
                    }
                    start = DateTime.Now;
                    UO.Cast("Fireball", Aliases.LastAttack);
                    while ((DateTime.Now - start) < TimeSpan.FromMilliseconds(SpellsDelays["Fireball"]))
                    {
                        if (!autocast) return;
                        UO.Wait(50);
                    }

                }
            }
            else if (new[] { "Lightning", "Bolt" }.Contains(spellname))
            {
                for (int i = 0; i < 3; i++)
                {
                    if (!autocast)
                    {
                        UO.Print("Autocast OFF");
                        return;
                    }
                    start = DateTime.Now;
                    UO.Cast("Lightning", Aliases.LastAttack);
                    while ((DateTime.Now - start) < TimeSpan.FromMilliseconds(SpellsDelays["Lightning"]))
                    {
                        if (!autocast) return;
                        UO.Wait(50);
                    }

                }
            }
            else if (new[] { "Harm", "Mind" }.Contains(spellname))
            {
                for (int i = 0; i < 3; i++)
                {
                    if (!autocast)
                    {
                        UO.Print("Autocast OFF");
                        return;
                    }
                    start = DateTime.Now;
                    UO.Cast("Harm", Aliases.LastAttack);
                    while ((DateTime.Now - start) < TimeSpan.FromMilliseconds(SpellsDelays["Harm"]))
                    {
                        if (!autocast) return;
                        UO.Wait(50);
                    }

                }
            }
        }




        #endregion
    }
}
