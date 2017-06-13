using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Phoenix;

namespace Mining.GUI
{
    [PhoenixWindowTabPage("Mining")]
    public partial class Mining : UserControl
    {
        public static Mining LastInstance;
        public event EventHandler OnCHanged;

        public Mining()
        {

            InitializeComponent();
            LastInstance = this;

            // Buttons
            btn_BankPos.Click += Changed;
            btn_GemBox.Click += Changed;
            btn_RecallPos.Click += Changed;
            btn_ResourceBox.Click += Changed;
            btn_SelectMine.Click += Changed;
            btn_SetDoor.Click += Changed;
            btn_SetWeapon.Click += Changed;
            btn_StartMine.Click += Changed;
            btn_OreBox.Click += Changed;

            lbl_AAdamantium.TextChanged += Changed;
            lbl_AIron.TextChanged += Changed;
            lbl_AObsidian.TextChanged += Changed;
            lbl_ASilicon.TextChanged += Changed;
            lbl_AValorite.TextChanged += Changed;
            lbl_AVerite.TextChanged += Changed;
            lbl_TAdamantium.TextChanged += Changed;
            lbl_TIron.TextChanged += Changed;
            lbl_TObsidian.TextChanged += Changed;
            lbl_TSilicon.TextChanged += Changed;
            lbl_TValorite.TextChanged += Changed;
            lbl_TVerite.TextChanged += Changed;
            lb_Mines.TextChanged += Changed;

            chb_AutoRemoveRocks.CheckedChanged += Changed;
            chb_DropCopper.CheckedChanged += Changed;
            chb_SkipCopper.CheckedChanged += Changed;
            chb_SkipIron.CheckedChanged += Changed;
            chb_SkipSilicon.CheckedChanged += Changed;
            chb_SkipVerite.CheckedChanged += Changed;
            chb_UseCrystal.CheckedChanged += Changed;

            tb_FightSay.TextChanged += Changed;

            numAda.ValueChanged += Changed;
            numObs.ValueChanged += Changed;

            



        }

        private void Changed(object sender, EventArgs e)
        {
            EventHandler temp = OnCHanged;
            if(temp!=null)
            {
                foreach(EventHandler ev in temp.GetInvocationList())
                {
                    ev.BeginInvoke(sender, null, null, null);
                }
            }
        }

        // Labels

        public string AIron
        {
            get
            {
                return lbl_AIron.Text;
            }
            set
            {
                lbl_AIron.Text = value;
            }
        }
        public string ASilicon
        {
            get
            {
                return lbl_ASilicon.Text;
            }
            set
            {
                lbl_ASilicon.Text = value;
            }
        }

        public string AVerite
        {
            get
            {
                return lbl_AVerite.Text;
            }
            set
            {
                lbl_AVerite.Text = value;
            }
        }

        public string AValorite
        {
            get
            {
                return lbl_AValorite.Text;
            }
            set
            {
                lbl_AValorite.Text = value;
            }
        }

        public string AObsidian
        {
            get
            {
                return lbl_AObsidian.Text;
            }
            set
            {
                lbl_AObsidian.Text = value;
            }
        }

        public string AAdamantium
        {
            get
            {
                return lbl_AAdamantium.Text;
            }
            set
            {
                lbl_AAdamantium.Text = value;
            }
        }


        public string TIron
        {
            get
            {
                return lbl_TIron.Text;
            }
            set
            {
                lbl_TIron.Text = value;
            }
        }

        public string TSilicon
        {
            get
            {
                return lbl_TSilicon.Text;
            }
            set
            {
                lbl_TSilicon.Text = value;
            }
        }

        public string TVerite
        {
            get
            {
                return lbl_TVerite.Text;
            }
            set
            {
                lbl_TVerite.Text = value;
            }
        }

        public string TValorite
        {
            get
            {
                return lbl_TValorite.Text;
            }
            set
            {
                lbl_TValorite.Text = value;
            }
        }

        public string TObsidian
        {
            get
            {
                return lbl_TObsidian.Text;
            }
            set
            {
                lbl_TObsidian.Text = value;
            }
        }

        public string TAdamantium
        {
            get
            {
                return lbl_TAdamantium.Text;
            }
            set
            {
                lbl_TAdamantium.Text = value;
            }
        }

        // Progress Bar
        public ProgressBar WeightProgress
        {
            get
            {
                return pb_Weight;
            }
        }

        // Check Boxes

        public bool DropCopper
        {
            get
            {
                return chb_DropCopper.Checked;
            }
            set
            {
                chb_DropCopper.Checked = value;
            }
        }

        public bool SkipCopper
        {
            get
            {
                return chb_SkipCopper.Checked;
            }
            set
            {
                chb_SkipCopper.Checked = value;
            }
        }

        public bool SkipIron
        {
            get
            {
                return chb_SkipIron.Checked;
            }
            set
            {
                chb_SkipIron.Checked = value;
            }
        }

        public bool SkipSilicon
        {
            get
            {
                return chb_SkipSilicon.Checked;
            }
            set
            {
                chb_SkipSilicon.Checked = value;
            }
        }

        public bool SkipVerite
        {
            get
            {
                return chb_SkipVerite.Checked;
            }
            set
            {
                chb_SkipVerite.Checked = value;
            }
        }

        public bool AutoRemoveRocks
        {
            get
            {
                return chb_AutoRemoveRocks.Checked;
            }
            set
            {
                chb_AutoRemoveRocks.Checked = value;
            }
        }

        public bool UseCrystal
        {
            get
            {
                return chb_UseCrystal.Checked;
            }
            set
            {
                chb_UseCrystal.Checked = value;
            }
        }

        // Text Box
        public string FightSay
        {
            get
            {
                return tb_FightSay.Text;
            }
            set
            {
                tb_FightSay.Text = value;
            }
        }

        // Numeric
        public decimal MaxObs
        {
            get
            {
                return numObs.Value;
            }
            set
            {
                numObs.Value = value;
            }
        }

        public decimal MaxAda
        {
            get
            {
                return numAda.Value;
            }
            set
            {
                numAda.Value = value;
            }
        }

        // Listbox

        public ListBox Mines
        {
            get
            {
                return lb_Mines;
            }
        }



  
    }
}
