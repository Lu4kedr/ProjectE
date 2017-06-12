using Phoenix;
using Phoenix.WorldData;
using Project_E.Lib.WeaponsSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Project_E.Lib.Watcher;

namespace Project_E
{
    public class Commands
    {
        private bool Register = true;
        private bool HealON = false;
        private UOCharacter provo1;
        private UOCharacter provo2;
        DateTime HiddenTime;
        public Commands()
        {
            

        }

        private void WT_HiddenChanged(object sender, HiddenChangedArgs e)
        {
            if (e.State) HiddenTime = DateTime.Now;
            if (!e.State) hidoff();
        }

        [Command]
        public void heal()
        {
            if (HealON)
            {
                Main.Instance.AH.Stop();
                HealON = false;
                UO.PrintError("Heal Off");
            }
            else
            {
                Main.Instance.AH.Start();
                HealON = true;
                UO.PrintInformation("Heal On");
            }
        }


        [Command]
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
                    if (t.Weapon == Main.Instance.SGUI.Weapons.ActualWeapon.Weapon && t.Shield == Main.Instance.SGUI.Weapons.ActualWeapon.Shield)
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


        [Command]
        public void peace()
        {
            Main.Instance.SK.Peace_Entic(StandardSkill.Peacemaking, Aliases.GetObject("laststatus"), ref Main.Instance.AH.MusicDone);
        }

        [Command]
        public void entic()
        {
            Main.Instance.SK.Peace_Entic(StandardSkill.Discordance_Enticement, Aliases.GetObject("laststatus"), ref Main.Instance.AH.MusicDone);
        }


        [Command]
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

        [Command]
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
        public void obet()
        {
            Main.Instance.AB.Sacrafire(bandage);
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

        [Command]
        public void ccast(string spellname, Serial target)
        {
            Main.Instance.SK.ccast(spellname, target, bandage, obet);

        }

        [Command]
        public void nhcast(string spellname, Serial target)
        {
            Main.Instance.SK.ccast(spellname, target, bandage, obet, Main.Instance.AH);

        }

        [Command]
        public void inv()
        {
            Main.Instance.SK.Invis(bandage, obet);

        }


        [Command]
        public void reactiv(Serial target)
        {
            Main.Instance.SK.ReactiveArmor(target);

        }

    }
}
