using Phoenix;
using System;
using System.Windows.Forms;

namespace Project_E.GUI
{
    [PhoenixWindowTabPage("Erebor")]
    public partial class EreborGUI : UserControl
    {
        public event EventHandler<OnChangedArgs> OnChanged;
        public static EreborGUI LastInstance;

        public EreborGUI()
        {
            InitializeComponent();

            // Register event 

            // Lot settings
            chb_Bolts.CheckedChanged += Changed;
            chb_Feathers.CheckedChanged += Changed;
            chb_Food.CheckedChanged += Changed;
            chb_Gems.CheckedChanged += Changed;
            chb_Leather.CheckedChanged += Changed;
            chb_Regeants.CheckedChanged += Changed;

            btn_LotOnOff.Click += Changed;
            btn_SetCarvTool.Click += Changed;
            btn_SetItem1.Click += Changed;
            btn_SetItem2.Click += Changed;
            btn_SetLotBackpack.Click += Changed;



            // Values Settings
            tb_GoldLimit.TextChanged += Changed;
            tb_Hits2Drink.TextChanged += Changed;
            tb_HiddenDelay.TextChanged += Changed;
            tb_Sacrafire.TextChanged += Changed;
            tb_Hits2Bandage.TextChanged += Changed;
            cb_PriestShaman.TextChanged += Changed;
            tb_Width.TextChanged += Changed;
            tb_Height.TextChanged += Changed;



            // Other Settings
            chb_CorpsesHide.CheckedChanged += Changed;
            chb_AutoMorf.CheckedChanged += Changed;
            chb_HitBandage.CheckedChanged += Changed;
            chb_HitTracking.CheckedChanged += Changed;
            chb_AutoHarm.CheckedChanged += Changed;
            chb_AutoDrink.CheckedChanged += Changed;
            chb_PrintStoods.CheckedChanged += Changed;

            btn_SetPoison.Click += Changed;



            // Hotkeys
            btn_AddHotkey.Click += Changed;
            btn_ClearHotkeys.Click += Changed;



            // Lists
            btn_0.Click += Changed;
            btn_1.Click += Changed;
            btn_2.Click += Changed;
            btn_3.Click += Changed;
            btn_4.Click += Changed;
            btn_5.Click += Changed;


            // Tab Control
            TabControl.SelectedIndexChanged += TabControlChanged;



            LastInstance = this;

        }

        private void Changed(object sender, EventArgs e)
        {
            EventHandler<OnChangedArgs> temp = OnChanged;
            if (temp != null)
            {
                foreach (EventHandler<OnChangedArgs> ev in OnChanged.GetInvocationList())
                {
                    ev.BeginInvoke(sender, new OnChangedArgs() { TabControlSelectedID=TabControlIndex} , null, null);
                }
            }
        }


        // Lot Settings

        public bool Food
        {
            get
            {
                return chb_Food.Checked;

            }

            set
            {
                chb_Food.Checked = value;
            }
        }

        public bool AutoSacrafire
        {
            get
            {
                return chb_AutoSacrafire.Checked;
            }
            set
            {
                chb_AutoSacrafire.Checked = value;
            }
        }

        public bool Leather
        {
            get
            {
                return chb_Leather.Checked;
            }

            set
            {
                chb_Leather.Checked = value;
            }
        }

        public bool Bolts
        {
            get
            {
                return chb_Bolts.Checked;
            }

            set
            {
                chb_Bolts.Checked = value;
            }
        }

        public ushort Extend1
        {
            get
            {

                return ushort.Parse(tb_AddItem1.Text ?? "0");
            }

            set
            {
                tb_AddItem1.Text = value.ToString();
            }
        }

        public ushort Extend2
        {
            get
            {

                return ushort.Parse(tb_AddItem2.Text ?? "0");
            }

            set
            {
                tb_AddItem2.Text = value.ToString();
            }
        }

        public bool Feathers
        {
            get
            {
                return chb_Feathers.Checked;
            }

            set
            {
                chb_Feathers.Checked = value;
            }
        }

        public bool Gems
        {
            get
            {
                return chb_Gems.Checked;
            }

            set
            {
                chb_Gems.Checked = value;
            }
        }

        public bool Regeants
        {
            get
            {
                return chb_Regeants.Checked;
            }

            set
            {
                chb_Regeants.Checked = value;
            }
        }

        public Button LotOnOff
        {
            get
            {
                return btn_LotOnOff;
            }

        }



        // Values Settings

        public string GoldLimit
        {
            get
            {
                return tb_GoldLimit.Text ?? "0";
            }
            set
            {
                tb_GoldLimit.Text = value;
            }
        }

        public string Hits2Drink
        {
            get
            {
                return tb_Hits2Drink.Text ?? "0";
            }
            set
            {
                tb_Hits2Drink.Text = value;
            }
        }

        public string HiddingDelay
        {
            get
            {
                return tb_HiddenDelay.Text ?? "0";
            }
            set
            {
                tb_HiddenDelay.Text = value;
            }
        }

        public string Sacrafire
        {
            get
            {
                return tb_Sacrafire.Text ?? "0";
            }
            set
            {
                tb_Sacrafire.Text = value;
            }
        }

        public string Hits2Bandage
        {
            get
            {
                return tb_Hits2Bandage.Text ?? "0";
            }
            set
            {
                tb_Hits2Bandage.Text = value;
            }
        }

        public int KlerikShaman
        {
            get
            {
                return cb_PriestShaman.SelectedIndex;
            }
            set
            {
                cb_PriestShaman.SelectedIndex = value;
            }
        }

        public string GW_Width
        {
            get
            {
                return tb_Width.Text ?? "0";
            }
            set
            {
                tb_Width.Text = value;
            }
        }

        public string GW_Height
        {
            get
            {
                return tb_Height.Text ?? "0";
            }
            set
            {
                tb_Height.Text = value;
            }
        }


        // Other Settings

        public bool CorpsesHide
        {
            get
            {
                return chb_CorpsesHide.Checked;
            }
            set
            {
                chb_CorpsesHide.Checked = value;
            }
        }

        public bool AutoMorf
        {
            get
            {
                return chb_AutoMorf.Checked;
            }
            set
            {
                chb_AutoMorf.Checked = value;
            }
        }

        public bool HitBandage
        {
            get
            {
                return chb_HitBandage.Checked;
            }
            set
            {
                chb_HitBandage.Checked = value;
            }
        }

        public bool HitTracking
        {
            get
            {
                return chb_HitTracking.Checked;
            }
            set
            {
                chb_HitTracking.Checked = value;
            }
        }

        public bool AutoHarm
        {
            get
            {
                return chb_AutoHarm.Checked;
            }
            set
            {
                chb_AutoHarm.Checked = value;
            }
        }

        public bool AutoDrink
        {
            get
            {
                return chb_AutoDrink.Checked;
            }
            set
            {
                chb_AutoDrink.Checked = value;
            }
        }

        public bool PrintStoodUp
        {
            get
            {
                return chb_PrintStoods.Checked;
            }
            set
            {
                chb_PrintStoods.Checked = value;
            }
        }

        public Button Poison
        {
            get
            {
                return btn_SetPoison;
            }
        }

        // Lists

        public TreeView Runes
        {
            get
            {
                return tw_Runes;
            }
        }

        public ListView Weapons
        {
            get
            {
                return lw_Weapons;
            }
        }

        public ListView Equips
        {
            get
            {
                return lw_Equips;
            }
        }

        public ListView HealedPlayers
        {
            get
            {
                return lw_Patients;
            }
        }

        public ListView TrackIgnore
        {
            get
            {
                return lw_TrackIgnored;
            }
        }

        public TabControl TabControl
        {
            get
            {
                return TabControl_;
            }
        }
        public int TabControlIndex
        {
            get
            {
                return TabControl.SelectedIndex;
            }
        }

        // Tab Changed

        private void TabControlChanged(object sender, EventArgs e)
        {
            switch (TabControl.SelectedIndex)
            {
                // Runes
                case 0:
                    btn_0.Visible = true;
                    btn_0.Text = "Recall - Svitek";

                    btn_1.Visible = true;
                    btn_1.Text = "Refresh";

                    btn_2.Visible = true;
                    btn_2.Text = "Recall";

                    btn_3.Visible = true;
                    btn_3.Text = "Scan";

                    btn_4.Visible = true;
                    btn_4.Text = "Gate";

                    btn_5.Visible = false;
                    btn_5.Text = "text";

                    break;

                // Weapons
                case 1:
                    btn_0.Visible = true;
                    btn_0.Text = "Refresh";

                    btn_1.Visible = false;
                    btn_1.Text = "";

                    btn_2.Visible = true;
                    btn_2.Text = "Add";

                    btn_3.Visible = false;
                    btn_3.Text = "text";

                    btn_4.Visible = true;
                    btn_4.Text = "Delete";

                    btn_5.Visible = false;
                    btn_5.Text = "text";
                    break;

                // Equips
                case 2:
                    btn_0.Visible = true;
                    btn_0.Text = "Dress";

                    btn_1.Visible = true;
                    btn_1.Text = "Refresh";

                    btn_2.Visible = true;
                    btn_2.Text = "Dress & Undress";

                    btn_3.Visible = true;
                    btn_3.Text = "Add";

                    btn_4.Visible = false;
                    btn_4.Text = "text";

                    btn_5.Visible = true;
                    btn_5.Text = "Delete";
                    break;

                // Healed
                case 3:
                    btn_0.Visible = true;
                    btn_0.Text = "Refresh";

                    btn_1.Visible = false;
                    btn_1.Text = "";

                    btn_2.Visible = true;
                    btn_2.Text = "Add";

                    btn_3.Visible = false;
                    btn_3.Text = "text";

                    btn_4.Visible = true;
                    btn_4.Text = "Delete";

                    btn_5.Visible = false;
                    btn_5.Text = "text";
                    break;

                // Track Ignored
                case 4:
                    btn_0.Visible = true;
                    btn_0.Text = "Refresh";

                    btn_1.Visible = false;
                    btn_1.Text = "";

                    btn_2.Visible = true;
                    btn_2.Text = "Add";

                    btn_3.Visible = false;
                    btn_3.Text = "text";

                    btn_4.Visible = true;
                    btn_4.Text = "Delete";

                    btn_5.Visible = false;
                    btn_5.Text = "text";
                    break;
            }
        }
    }
}
