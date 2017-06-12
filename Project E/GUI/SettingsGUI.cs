using Project_E.Lib;
using Project_E.Lib.EquipSet;
using Project_E.Lib.Healing;
using Project_E.Lib.Runes;
using Project_E.Lib.Skills;
using Project_E.Lib.WeaponsSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Project_E.GUI
{
    [Serializable]
    public class SettingsGUI
    {
        // Default Values
        public SettingsGUI()
        {
            Food = false;
            Leather = false;
            Bolts = false;
            Extend1 = 0;
            Extend2 = 0;
            Feathers = false;
            Gems = true;
            Regeants = true;
            GoldLimit = "5000";
            Hits2Drink = "50";
            HiddingDelay = "2800";
            Sacrafire = "40";
            Hits2Bandage = "80";
            KlerikShaman = 0;
            CorpsesHide = false;
            AutoMorf = false;
            HitBandage = false;
            HitTracking = false;
            AutoHarm = true;
            AutoDrink = true;
            PrintStoodUp = false;
            LotBackpack = 0;
            CarvTool = 0;
            Poison = 0;
            AutoSacrafire = true;
            HealedPlayers = new HealedPlayers();
            Weapons = new Weapons();
            Equips = new EquipSet();
            Runes = new RuneTree();
            Tracking = new Tracking();
            SwitchabeHotkeys = new SwitchabeHotkeys();

        }

        // Settings

        public bool AutoSacrafire
        {
            get
            {
                bool tmp = false;
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    tmp = EreborGUI.LastInstance.AutoSacrafire;
                }));
                return tmp;
            }
            set
            {
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    EreborGUI.LastInstance.AutoSacrafire = value;
                }));
            }
        }
        public bool Food
        {
            get
            {
                bool tmp = false;
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    tmp = EreborGUI.LastInstance.Food;
                }));
                return tmp;

            }

            set
            {
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    EreborGUI.LastInstance.Food = value;
                }));
            }
        }

        public bool Leather
        {
            get
            {
                bool tmp = false;
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    tmp = EreborGUI.LastInstance.Leather;
                }));
                return tmp;
            }

            set
            {
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    EreborGUI.LastInstance.Leather = value;
                }));
            }
        }

        public bool Bolts
        {
            get
            {
                bool tmp = false;
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    tmp = EreborGUI.LastInstance.Bolts;
                }));
                return tmp;

            }

            set
            {
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    EreborGUI.LastInstance.Bolts = value;
                }));
            }
        }

        public ushort Extend1
        {
            get
            {
                ushort tmp = 0;
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    tmp = EreborGUI.LastInstance.Extend1;
                }));
                return tmp;
            }

            set
            {
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    EreborGUI.LastInstance.Extend1 = value;
                }));
            }
        }

        public ushort Extend2
        {
            get
            {
                ushort tmp = 0;
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    tmp = EreborGUI.LastInstance.Extend2;
                }));
                return tmp;
            }

            set
            {
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    EreborGUI.LastInstance.Extend2 = value;
                }));
            }
        }

        public bool Feathers
        {
            get
            {
                bool tmp = false;
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    tmp = EreborGUI.LastInstance.Feathers;
                }));
                return tmp;
            }

            set
            {
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    EreborGUI.LastInstance.Feathers = value;
                }));
            }
        }

        public bool Gems
        {
            get
            {
                bool tmp = true;
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    tmp = EreborGUI.LastInstance.Gems;
                }));
                return tmp;
            }

            set
            {
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    EreborGUI.LastInstance.Gems = value;
                }));
            }
        }

        public bool Regeants
        {
            get
            {
                bool tmp = true;
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    tmp = EreborGUI.LastInstance.Regeants;
                }));
                return tmp;
            }

            set
            {
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    EreborGUI.LastInstance.Regeants = value;
                }));
            }
        }

        public string GoldLimit
        {
            get
            {
                string tmp = "5000";
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    tmp = EreborGUI.LastInstance.GoldLimit;
                }));
                return tmp;
            }

            set
            {
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    EreborGUI.LastInstance.GoldLimit = value;
                }));
            }
        }

        public string Hits2Drink
        {
            get
            {
                string tmp = "50";
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    tmp = EreborGUI.LastInstance.Hits2Drink;
                }));
                return tmp;
            }

            set
            {
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    EreborGUI.LastInstance.Hits2Drink = value;
                }));
            }
        }

        public string HiddingDelay
        {
            get
            {
                string tmp = "2800";
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    tmp = EreborGUI.LastInstance.HiddingDelay;
                }));
                return tmp;
            }

            set
            {
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    EreborGUI.LastInstance.HiddingDelay = value;
                }));
            }
        }

        public string Sacrafire
        {
            get
            {
                string tmp = "40";
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    tmp = EreborGUI.LastInstance.Sacrafire;
                }));
                return tmp;
            }

            set
            {
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    EreborGUI.LastInstance.Sacrafire = value;
                }));
            }
        }

        public string Hits2Bandage
        {
            get
            {
                string tmp = "80";
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    tmp = EreborGUI.LastInstance.Hits2Bandage;
                }));
                return tmp;
            }

            set
            {
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    EreborGUI.LastInstance.Hits2Bandage = value;
                }));
            }
        }

        public int KlerikShaman
        {
            get
            {
                int tmp = 0;
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    tmp = EreborGUI.LastInstance.KlerikShaman;
                }));
                return tmp;
            }

            set
            {
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    EreborGUI.LastInstance.KlerikShaman = value;
                }));
            }
        }

        public bool CorpsesHide
        {
            get
            {
                bool tmp = false;
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    tmp = EreborGUI.LastInstance.CorpsesHide;
                }));
                return tmp;
            }

            set
            {
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    EreborGUI.LastInstance.CorpsesHide = value;
                }));
            }
        }

        public bool AutoMorf
        {
            get
            {
                bool tmp = false;
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    tmp = EreborGUI.LastInstance.AutoMorf;
                }));
                return tmp;
            }

            set
            {
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    EreborGUI.LastInstance.AutoMorf = value;
                }));
            }
        }

        public bool HitBandage
        {
            get
            {
                bool tmp = false;
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    tmp = EreborGUI.LastInstance.HitBandage;
                }));
                return tmp;
            }

            set
            {
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    EreborGUI.LastInstance.HitBandage = value;
                }));
            }
        }

        public bool HitTracking
        {
            get
            {
                bool tmp = false;
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    tmp = EreborGUI.LastInstance.HitTracking;
                }));
                return tmp;
            }

            set
            {
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    EreborGUI.LastInstance.HitTracking = value;
                }));
            }
        }

        public bool AutoHarm
        {
            get
            {
                bool tmp = true;
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    tmp = EreborGUI.LastInstance.AutoHarm;
                }));
                return tmp;
            }

            set
            {
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    EreborGUI.LastInstance.AutoHarm = value;
                }));
            }
        }

        public bool AutoDrink
        {
            get
            {
                bool tmp = true;
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    tmp = EreborGUI.LastInstance.AutoDrink;
                }));
                return tmp;
            }

            set
            {
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    EreborGUI.LastInstance.AutoDrink = value;
                }));
            }
        }

        public bool PrintStoodUp
        {
            get
            {
                bool tmp = false;
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    tmp = EreborGUI.LastInstance.PrintStoodUp;
                }));
                return tmp;
            }

            set
            {
                EreborGUI.LastInstance.Invoke(new MethodInvoker(delegate
                {
                    EreborGUI.LastInstance.PrintStoodUp = value;
                }));
            }
        }

        public uint LotBackpack
        {
            get; set;
        }
        public uint CarvTool
        {
            get; set;
        }

        public ushort Poison
        { get; set; }


        public HealedPlayers HealedPlayers
        {
            get; set;
        }

        public Weapons Weapons
        {
            get; set;
        }

        public EquipSet Equips
        {
            get; set;
        }

        public RuneTree Runes { get; set; }

        public Tracking Tracking { get; set; }

        public SwitchabeHotkeys SwitchabeHotkeys
        {
            get;set;
        }
    }

}
