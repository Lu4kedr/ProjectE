using Phoenix;
using Phoenix.Communication;
using Phoenix.Communication.Packets;
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
            UO.Warmode(false);
            zbran.WaitTarget();
            pois.Use();
        }


        #endregion

        #region Veterinary
        UOCharacter pet =null;
        bool healed = false;
        bool onoff = false;
        bool harmed = false;



        public void autoVet()
        {
            if (!onoff)
            {
                onoff = true;
                UO.PrintWarning("Bandim do full");
            }
            else
            {
                onoff = false;
                UO.PrintWarning("Bandeni vypnuto");
            }
            while (onoff)
            {
                if (!Vet())
                {
                    UO.PrintError("Vypinám");
                    onoff = false;
                    return;
                }

            }
        }
        public bool Vet()
        {
            if (pet == null || pet.Distance > 15)
            {
                UO.Print("Zamer mezlicka");
                pet = new UOCharacter(UIManager.TargetObject());
            }
            if (pet == null) return false;
            if (pet.Distance > 6)
            {
                UO.PrintError("Moc daleko");
                return false;
            }
            if (UIManager.CurrentState != UIManager.State.Ready)
            {
                UO.Wait(200);
                return true;
            }
            Core.UnregisterServerMessageCallback(0x1C, onHeal);
            Core.RegisterServerMessageCallback(0x1C, onHeal);
            //Osetreni se ti nepovedlo.
            //byl uspesne osetren.
            // neni zranen.

            pet.WaitTarget();
            UO.Say(".bandage");
            healed = false;
            harmed = true;
            DateTime start = DateTime.Now;
            Settings.Weapons.ActualWeapon.Equip();
            while (!healed)
            {
                UO.Wait(100);
                if (DateTime.Now - start > TimeSpan.FromSeconds(6)) break;
                if (!harmed)
                {
                    UO.PrintInformation("Neni zranen");
                    Core.UnregisterServerMessageCallback(0x1C, onHeal);
                    return false;
                }
            }
            Core.UnregisterServerMessageCallback(0x1C, onHeal);
            return true;
        }

        CallbackResult onHeal(byte[] data, CallbackResult prevResult)
        {
            AsciiSpeech packet = new AsciiSpeech(data);
            if (packet.Text.Contains(" byl uspesne osetren.") || packet.Text.Contains("Osetreni se ti nepovedlo."))
            {
                healed = true;
            }
            if (packet.Text.Contains(" neni zranen."))
            {
                healed = true;
                harmed = false;
            }
            return CallbackResult.Normal;
        }
#endregion

        #region Hiding
        private bool getHit;
        public void Hiding(bool first)
        {
            bandage b = new bandage(Main.Instance.AH.Bandage);
            Core.UnregisterClientMessageCallback(0x02, ForceWalk);
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
                    
                    Core.UnregisterClientMessageCallback(0x02, ForceWalk);
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

                    Core.RegisterClientMessageCallback(0x02, ForceWalk, CallbackPriority.Highest);
                }
                UO.Wait(10);
            }
            if (Journal.WaitForText(true, 300, "nepovedlo se ti schovat.", "skryti se povedlo."))
            {
                UO.Wait(100);
                if (!World.Player.Hidden)
                    Core.UnregisterClientMessageCallback(0x02, ForceWalk);
                else
                    b.BeginInvoke(null, null);
            }
            else
            {
                Core.UnregisterClientMessageCallback(0x02, ForceWalk);

            }
            Main.Instance.WT.HitsChanged -= WT_HitsChanged;

        }

        private void WT_HitsChanged(object sender, Watcher.HitsChangedArgs e)
        {
            if (!e.gain) getHit = true;
        }

        public void hidoff()
        {
            Core.UnregisterClientMessageCallback(0x02, ForceWalk);
        }

        CallbackResult ForceWalk(byte[] data, CallbackResult prev)//0x02 clientReq
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
                return CallbackResult.Eat;
            }
            
            return CallbackResult.Normal;
        }


        #endregion

        #region Magery

        Dictionary<string, int> SpellsDelays = new Dictionary<string, int>
        {
            { "Fireball", 2060 },
            { "Flame", 4100 },
            { "Meteor", 6750 },
            { "Lightning", 3550 },
            { "Bolt", 3100 },
            { "frostbolt", 3000 },
            { "Harm", 1500 },
            { "Mind", 3450 },
            { "Invis", 3300 }
        };




        public void ReactiveArmor(Serial target)
        {
            DateTime start;
            UO.Warmode(false);
            if (World.Player.Backpack.AllItems.FindType(0x1F2D).Amount > 0)
            {
                new UOObject(target).WaitTarget();
                World.Player.Backpack.AllItems.FindType(0x1F2D).Use();
            }
            else
                UO.Cast(StandardSpell.ReactiveArmor, target);
            if (!Journal.WaitForText(true, 2000, "Kouzlo se nezdarilo."))
            {
                start = DateTime.Now;
            }
            else return;
            while (DateTime.Now - start < TimeSpan.FromSeconds(9)) UO.Wait(100);
            UO.PrintError("Reactiv vyprsel");
        }

        public void Teleport(Serial target)
        {
            if(new UOCharacter(target).Distance>12)
            {
                UO.PrintError("Moc daleko");
                return;
            }
            DateTime start;
            UO.Warmode(false);
            if (World.Player.Backpack.AllItems.FindType(0x1F42).Amount > 0)
            {
                new UOObject(target).WaitTarget();
                World.Player.Backpack.AllItems.FindType(0x1F42).Use();
            }
            else
                UO.Cast(StandardSpell.Teleport, target);
            if (!Journal.WaitForText(true, 3300, "Kouzlo se nezdarilo."))
            {
                start = DateTime.Now;
            }
            else return;
            UO.Attack(target);
        }

        public void Invis()
        {
            //int tmp = 8;
            UO.Warmode(false);
            UO.Cast("Invis", Aliases.Self);

            if (Journal.WaitForText(true, SpellsDelays["Invis"] / 3, "Kouzlo se nezdarilo.")) return;
            World.Player.Print(3);
            if (Journal.WaitForText(true, SpellsDelays["Invis"] / 3, "Kouzlo se nezdarilo.")) return;
            World.Player.Print(2);
            if (!Journal.WaitForText(true, SpellsDelays["Invis"] / 3, "Kouzlo se nezdarilo."))
            {
                World.Player.Print(1);
                //if (clearPerimeter(2))
                //{
                //    DateTime start = DateTime.Now;
                //    UO.PrintInformation("Blokuju krok na 1 s");
                //    Core.UnregisterClientMessageCallback(0x02, blockStep);
                //    Core.RegisterClientMessageCallback(0x02, blockStep);
                //    while ((DateTime.Now - start) < TimeSpan.FromMilliseconds(1000))
                //    {
                //        UO.Wait(50);
                //        tmp--;
                //        if (!clearPerimeter(2))
                //        {
                //            Core.UnregisterClientMessageCallback(0x02, blockStep);
                //            return;
                //        }
                //        if (tmp < 0 && !World.Player.Hidden)
                //        {
                //            Core.UnregisterClientMessageCallback(0x02, blockStep);
                //            return;
                //        }
                //    }
                //    Core.UnregisterClientMessageCallback(0x02, blockStep);
                //}
            }
        }
        //private bool clearPerimeter(int perimeterSize)
        //{
        //    foreach (UOCharacter ch in World.Characters.Where(cha => cha.Distance < perimeterSize && cha.Notoriety > Notoriety.Neutral))
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        public void HarmSelf(bool attacklast, bool war=false)
        {
            UO.Warmode(false);
            if (war)
            {
                UO.Cast(StandardSpell.MagicArrow, World.Player.Serial);
                UO.Wait(2200);
            }
            else
            {
                UO.Cast(StandardSpell.Harm, World.Player.Serial);
                UO.Wait(1600);
            }
            if (attacklast)
            {
                UO.Warmode(true);
                UO.Attack(Aliases.GetObject("laststatus").IsValid ? Aliases.GetObject("laststatus") : 0x00);
            }
        }




        public void Autocast(string Spell, bool charged, Serial target, ref bool acast)
        {
            int spellsNum = 5;
            int chargeNum = 3;
            bool firstCharge = true;
            if (acast)
            {
                acast = false;
                UO.PrintError("Autocast OFF");
            }
            else
            {
                acast = true;
                UO.PrintError("Autocast ON");
                UOCharacter targ = new UOCharacter(target);
                DateTime start = DateTime.Now;
                while (acast)
                {
                    if(targ.Hits < 1 || targ.Hits > 200 || targ.Dead || !targ.Exist)
                    {
                        acast = false;
                        UO.PrintError("Autocast OFF");
                        return;
                    }

                    switch(Spell)
                    {
                        case "Harm":
                            UO.Cast(StandardSpell.Harm, target);
                            UO.Wait(SpellsDelays[Spell]);
                            break;
                        case "Fireball":
                            UO.Cast(StandardSpell.Fireball, target);
                            UO.Wait(SpellsDelays[Spell]);
                            break;
                        case "Flame":
                            if (charged)
                            {
                                for (int i = 0; i < chargeNum; i++)
                                {
                                    if (!acast) return;
                                    UO.Cast(StandardSpell.Fireball, target);
                                    UO.Wait(SpellsDelays["Fireball"]);
                                    if (!firstCharge) break;
                                }

                                if (!acast) return;
                                UO.Cast(StandardSpell.FlameStrike, target);
                                UO.Wait(SpellsDelays[Spell]);

                            }
                            else
                            {
                                UO.Cast(StandardSpell.FlameStrike, target);
                                UO.Wait(SpellsDelays[Spell]);
                            }
                            break;
                        case "Meteor":
                            UO.Cast(StandardSpell.MeteorShower, target);
                            UO.Wait(SpellsDelays[Spell]);
                            break;
                        case "Bolt":
                            if (charged)
                            {
                                for (int i = 0; i < chargeNum; i++)
                                {
                                    if (!acast) return;
                                    UO.Cast(StandardSpell.Lightning, target);
                                    UO.Wait(SpellsDelays["Lightning"]);
                                    if (!firstCharge) break;
                                }
                                if (firstCharge) firstCharge = false;
                                for (int i = 0; i < spellsNum; i++)
                                {
                                    if (!acast) return;
                                    UO.Cast(StandardSpell.EnergyBolt, target);
                                    UO.Wait(SpellsDelays[Spell]);
                                }
                            }
                            else
                            {
                                UO.Cast(StandardSpell.EnergyBolt, target);
                                UO.Wait(SpellsDelays[Spell]);
                            }
                            break;

                        case "Mind":
                            if (charged)
                            {
                                for (int i = 0; i < chargeNum; i++)
                                {
                                    if (!acast) return;
                                    UO.Cast(StandardSpell.Harm, target);
                                    UO.Wait(SpellsDelays["Harm"]);
                                    if (!firstCharge) break;
                                }
                                if (firstCharge) firstCharge = false;
                                for (int i = 0; i < spellsNum; i++)
                                {
                                    if (!acast) return;
                                    UO.Cast(StandardSpell.MindBlast, target);
                                    UO.Wait(SpellsDelays[Spell]);
                                }
                            }
                            else
                            {
                                UO.Cast(StandardSpell.MindBlast, target);
                                UO.Wait(SpellsDelays[Spell]);
                            }
                            break;

                    }
                }
            }
        }




        //public void AutoCast(string spellname, Action bandage, Action Sacrafire, ref bool autocast)
        //{
        //    if (autocast)
        //    {
        //        autocast = false;
        //        UO.PrintError("Autocast OFF");
        //    }
        //    else
        //    {
        //        autocast = true;
        //        UO.PrintError("Autocast ON");
        //        DateTime start;
        //        var target = Aliases.LastAttack;
        //        while (autocast)
        //        {
        //            UO.Cast(spellname, target);
        //            start = DateTime.Now;
        //            while ((DateTime.Now - start) < TimeSpan.FromMilliseconds(SpellsDelays[spellname]))
        //            {
        //                if (!autocast) return;
        //                UO.Wait(50);
        //            }

        //        }

        //    }




        //}

        //// TODO Autocast

        //public void AutoCast_Charged(string spellname,Action bandage, Action Sacrafire, ref bool autocast)
        //{
        //    int counter = 0;
        //    if (autocast)
        //    {
        //        autocast = false;
        //        UO.PrintError("Autocast OFF");
        //    }
        //    else
        //    {
        //        autocast = true;
        //        UO.PrintError("Autocast ON");
        //        DateTime start;
        //        var target = Aliases.LastAttack;
        //        while (autocast)
        //        {
        //            if (counter % 5 == 0)
        //                ChargeSpell(spellname, ref autocast);
        //            UO.Cast(spellname, target);
        //            start = DateTime.Now;
        //            while ((DateTime.Now - start) < TimeSpan.FromMilliseconds(SpellsDelays[spellname]))
        //            {
        //                if (!autocast) return;
        //                UO.Wait(50);
        //            }
        //            counter++;

        //        }

        //    }

        //}

        //private void ChargeSpell(string spellname, ref bool autocast)
        //{
        //    DateTime start;
        //    if (new[] { "Fireball", "Flame", "Meteor" }.Contains(spellname))
        //    {
        //        for (int i = 0; i < 3; i++)
        //        {
        //            if (!autocast)
        //            {
        //                UO.Print("Autocast OFF");
        //                return;
        //            }
        //            start = DateTime.Now;
        //            UO.Cast("Fireball", Aliases.LastAttack);
        //            while ((DateTime.Now - start) < TimeSpan.FromMilliseconds(SpellsDelays["Fireball"]))
        //            {
        //                if (!autocast) return;
        //                UO.Wait(50);
        //            }

        //        }
        //    }
        //    else if (new[] { "Lightning", "Bolt" }.Contains(spellname))
        //    {
        //        for (int i = 0; i < 3; i++)
        //        {
        //            if (!autocast)
        //            {
        //                UO.Print("Autocast OFF");
        //                return;
        //            }
        //            start = DateTime.Now;
        //            UO.Cast("Lightning", Aliases.LastAttack);
        //            while ((DateTime.Now - start) < TimeSpan.FromMilliseconds(SpellsDelays["Lightning"]))
        //            {
        //                if (!autocast) return;
        //                UO.Wait(50);
        //            }

        //        }
        //    }
        //    else if (new[] { "Harm", "Mind" }.Contains(spellname))
        //    {
        //        for (int i = 0; i < 3; i++)
        //        {
        //            if (!autocast)
        //            {
        //                UO.Print("Autocast OFF");
        //                return;
        //            }
        //            start = DateTime.Now;
        //            UO.Cast("Harm", Aliases.LastAttack);
        //            while ((DateTime.Now - start) < TimeSpan.FromMilliseconds(SpellsDelays["Harm"]))
        //            {
        //                if (!autocast) return;
        //                UO.Wait(50);
        //            }

        //        }
        //    }
        //}




        #endregion


        #region Krafting


            #endregion
    }
}
