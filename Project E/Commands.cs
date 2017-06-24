using Phoenix;
using Phoenix.WorldData;
using Project_E.Lib.SpellManager;
using Project_E.Lib.WeaponsSet;
using System;
using System.Windows.Forms;
using static Project_E.Lib.Watcher;

namespace Project_E
{
    public delegate void bandage();
    public class Commands
    {
        private bool Register = true;
        private bool HealON = false;
        private UOCharacter provo1;
        private UOCharacter provo2;
        DateTime HiddenTime;
        private bool SpellFizz;

        public Commands()
        {
            

        }

        private void WT_HiddenChanged(object sender, HiddenChangedArgs e)
        {
            if (e.State) HiddenTime = DateTime.Now;
            if (!e.State) hidoff();
        }

        [Command]
        public void sipky()
        {
            Main.Instance.FA.sipky();
        }

        [Command]
        public void heal()
        {
            if (HealON)
            {
                HealON = false;
                //Main.Instance.AH.HealOnOff = false; 
               // Main.Instance.AH.AutoHealing(false);
                //UO.PrintWarning("Autoheal Off");


            }
            else
            {
                HealON = true;
                //Main.Instance.AH.HealOnOff = true;
                //UO.PrintWarning("Autoheal on");
                Main.Instance.AH.AutoHealing(ref HealON);


            }
        }


        [Command,BlockMultipleExecutions]
        public void res()
        {
            Main.Instance.AH.Res();
        }

        [Command("switch")]
        public void sw()
        {
            Main.Instance.SGUI.Weapons.SwitchWeapons();

            Main.Instance.EreborGUI.Invoke(new MethodInvoker(delegate
            {
                Main.Instance.EreborGUI.Weapons.Items.Clear();
                foreach (WeaponSet t in Main.Instance.SGUI.Weapons.weapons)
                {
                    ListViewItem lvitem = new ListViewItem(t.Name);
                    if (t.Weapon.Equals(Main.Instance.SGUI.Weapons.ActualWeapon.Weapon) && t.Shield.Equals(Main.Instance.SGUI.Weapons.ActualWeapon.Shield))
                    {
                        lvitem.SubItems.Add("Actual Weapon");
                    }
                    else
                    {
                        lvitem.SubItems.Add("- - -");
                    }
                    Main.Instance.EreborGUI.Weapons.Items.Add(lvitem);
                }
                Main.Instance.EreborGUI.Weapons.Refresh();
            }));
        }

        [Command]
        public void kuch()
        {
            Main.Instance.Autolot.Carving();
            Main.Instance.SGUI.Weapons.ActualWeapon.Equip();
        }

        [Command]
        public void track()
        {
            Main.Instance.SGUI.Tracking.Track();
        }
        [Command]
        public void track(int choice)
        {
            Main.Instance.SGUI.Tracking.Track(choice);
        }
        [Command]
        public void track(string var)
        {
            Main.Instance.SGUI.Tracking.Track(var);
        }
        [Command]
        public void track(int choice, bool war)
        {
            Main.Instance.SGUI.Tracking.Track(choice, war);
        }


        [Command]
        public void bandage()
        {
            Main.Instance.AH.Bandage();
        }

        [Command]
        public void war()
        {
            if (World.Player.Warmode)
            {
                UO.Warmode(false);
                UO.PrintInformation("Warmode off");
            }

            else
            {
                UO.Warmode(true);
                UO.PrintInformation("Warmode on");
            }
        }


        [Command, BlockMultipleExecutions]
        public void targetnext(bool closest)
        {
            Main.Instance.TRG.targetnext(true);
        }
        [Command, BlockMultipleExecutions]
        public void targetnext()
        {
            Main.Instance.TRG.targetnext();
        }


        [Command,BlockMultipleExecutions]
        public void peace()
        {
            Main.Instance.SK.Peace_Entic(StandardSkill.Peacemaking, Aliases.GetObject("laststatus"), ref Main.Instance.AH.MusicDone);
        }

        [Command,BlockMultipleExecutions]
        public void entic()
        {
            Main.Instance.SK.Peace_Entic(StandardSkill.Discordance_Enticement, Aliases.GetObject("laststatus"), ref Main.Instance.AH.MusicDone);
        }


        [Command,BlockMultipleExecutions]
        public void provo()
        {
            Main.Instance.SK.Provo(provo1, provo2, ref Main.Instance.AH.MusicDone);
        }


        [Command]
        public void setprovo()
        {
            UO.PrintWarning("Zamer Cil 1");
            provo1 = new UOCharacter(UIManager.TargetObject());
            UO.PrintWarning("Zamer Cil 2");
            provo2 = new UOCharacter(UIManager.TargetObject());
        }

        [Command, BlockMultipleExecutions]
        public void hid()
        {
            if(Register)
            {
                Register = false;
                Main.Instance.WT.HiddenChanged += WT_HiddenChanged;
            }
            Main.Instance.SK.Hiding(true);
        }

        [Command, BlockMultipleExecutions]
        public void hidoff()
        {
            Main.Instance.SK.hidoff();
        }

        [Command]
        public void pois()
        {
            Main.Instance.SK.Poisoning();
        }

        [Command, BlockMultipleExecutions]
        public void swhotkeys()
        {
            Main.Instance.SGUI.SwitchabeHotkeys.swHotk();
        }

        [Command]
        public void kudla()
        {
            Main.Instance.AB.kudla(Main.Instance.SGUI.Weapons);
        }

        [Command,BlockMultipleExecutions]
        public void probo()
        {
            Main.Instance.AB.probo(HiddenTime, hidoff);
        }

        [Command]
        public void friend()
        {
            Main.Instance.FA.friend();
        }


        [Command]
        public void kill()
        {
            Main.Instance.FA.kill();
        }


        [Command]
        public void sacrafire()
        {
            Main.Instance.AB.Sacrafire();
        }

        [Command]
        public void harmself()
        {
            Main.Instance.SK.HarmSelf(false);
        }
        [Command]
        public void harmself(bool attacklast)
        {
            Main.Instance.SK.HarmSelf(attacklast);
        }


        //[Command]
        //public void nhcast(string spellname, Serial target)
        //{
        //    Main.Instance.SM.OnSpellDone += SM_OnSpellDone;
        //    SpellFizz = false;
        //    //Aliases.SetObject("SpellTarget", target);
        //    if (HealON)
        //    {
        //        Main.Instance.AH.ShamanCast = true;
        //        if (spellname == "frostbolt" || spellname == "necrobolt")
        //        {
        //            UO.Say("." + spellname);
        //        }
        //        else
        //            UO.Cast(spellname);
        //        var StatSpell = DateTime.Now;
        //        int delay = 0;
        //        spellname = SpellManager.Name2Code(spellname);
        //        if (SpellManager.SpellDelays.ContainsKey(spellname))
        //        {
        //            delay = (SpellManager.SpellDelays[spellname] + 600);
        //        }
        //        while (!SpellFizz)
        //        {
        //            if (DateTime.Now - StatSpell > TimeSpan.FromMilliseconds(delay))
        //            {
        //                Main.Instance.SM.OnSpellDone -= SM_OnSpellDone;
        //                Main.Instance.AH.ShamanCast = false;
        //                break;
        //            }
        //            UO.Wait(100);
        //        }
        //        Main.Instance.AH.ShamanCast = false;
        //    }
        //    else
        //    if (spellname == "frostbolt" || spellname == "necrobolt")
        //    {
        //        UO.Say("." + spellname);
        //    }
        //    else
        //    {
        //        UO.Cast(spellname);
        //    }

        //}

        [Command]
        public void nhcast(string spellname, Serial target)
        {
            Main.Instance.SM.OnSpellDone += SM_OnSpellDone;
            SpellFizz = false;
            //Aliases.SetObject("SpellTarget", target);
            if (HealON)
            {
                Main.Instance.AH.ShamanCast = true;
                Main.Instance.SM.Cast(spellname, target);
                var StatSpell = DateTime.Now;
                int delay = 0;
                spellname = SpellManager.Name2Code(spellname);
                if (SpellManager.SpellDelays.ContainsKey(spellname))
                {
                    delay = (SpellManager.SpellDelays[spellname] + 300);
                }
                while (!SpellFizz)
                {
                    if (DateTime.Now - StatSpell > TimeSpan.FromMilliseconds(delay))
                    {
                        Main.Instance.SM.OnSpellDone -= SM_OnSpellDone;
                        Main.Instance.AH.ShamanCast = false;
                        break;
                    }
                    UO.Wait(100);
                }
                Main.Instance.AH.ShamanCast = false;
            }
            else
                Main.Instance.SM.Cast(spellname, target);


        }
        private void SM_OnSpellDone(object sender, SpellManager.OnSpellDoneArgs e)
        {
            Main.Instance.SM.OnSpellDone -= SM_OnSpellDone;
            //UO.Print("FIZZ");
            SpellFizz = true;
            UO.Wait(10);
            Main.Instance.SM.OnSpellDone += SM_OnSpellDone;

        }

        [Command]
        public void ccast(string spellname, Serial target)
        {
            //Aliases.SetObject("SpellTarget", target);
            //if (spellname == "frostbolt" || spellname == "necrobolt")
            //{
            //    UO.Say("." + spellname);
            //}
            //else
            //    UO.Cast(spellname);
            Main.Instance.SM.Cast(spellname, target);

        }

        



        [Command]
        public void inv()
        {
            Main.Instance.SK.Invis();

        }


        [Command]
        public void reactiv(Serial target)
        {
            Main.Instance.SK.ReactiveArmor(target);

        }

        [Command, BlockMultipleExecutions]
        public void autopetheal()
        {
            Main.Instance.SK.autoVet();
        }

        [Command, BlockMultipleExecutions]
        public void petheal()
        {
            Main.Instance.SK.Vet();
        }



        [Command, BlockMultipleExecutions]
        public void boostself(string type)
        {
            Main.Instance.AB.selfboost(type);
            UO.Print("DOne");
        }

        [Command, BlockMultipleExecutions]
        public void boost(string type)
        {
            Main.Instance.AB.boost(type);
            UO.Print("DOne");
        }

        [Command,BlockMultipleExecutions]
        public void vyber(int delay)
        {
            UO.PrintInformation("Zamer co chces vybrat");
            Serial target = UIManager.TargetObject();
            Main.Instance.FA.TakeAllFrom(target, delay);
        }

        [Command,BlockMultipleExecutions]
        public void dress(int index)
        {
            Main.Instance.SGUI.Equips.equipy[index].DressOnly();
        }
    }
}
