using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Phoenix;
using System.Drawing;

namespace Mining
{
    [Serializable]
    public class Settings
    {
        private Mining.GUI.Mining I;

        public Settings()
        {
            I = Mining.GUI.Mining.LastInstance;

            ActualMapIndex = 0;//GH
            Maps = new List<Map>();
            AIron = 0;
            ASilicon = 0;
            AVerite = 0;
            AValorite = 0;
            AObsidian = 0;
            AAdamantium = 0;
            MaxObs = 4;
            MaxAda = 1;
            DropCopper = true;
            SkipCopper = false;
            SkipIron = false;
            SkipSilicon = false;
            SkipVerite = false;
            AutoRemoveRocks = true;
            FightSay = "";
            UseCrystal = true;
        }

        public bool KryskaTrollAlarm
        {
            get
            {
                bool tmp = true;
                I.Invoke(new MethodInvoker(delegate
                {
                    tmp = I.KryskaTrollAlarm;
                }));
                return tmp;
            }
            set
            {
                I.Invoke(new MethodInvoker(delegate
                {
                    I.KryskaTrollAlarm = value;
                }));
            }
        }
        public int ActualMapIndex
        {
            get;set;
        }
        public List<Map> Maps { get; set; }
        public int AIron
        {
            get
            {
                int tmp = 0;
                I.Invoke(new MethodInvoker(delegate
                {
                    tmp = int.Parse(I.AIron ?? "0");
                }));
                return tmp;
            }
            set
            {
                I.Invoke(new MethodInvoker(delegate
                {
                    I.AIron = value.ToString();
                }));
            }
        }

        public int ASilicon
        {
            get
            {
                int tmp = 0;
                I.Invoke(new MethodInvoker(delegate
                {
                    tmp = int.Parse(I.ASilicon ?? "0");
                }));
                return tmp;
            }
            set
            {
                I.Invoke(new MethodInvoker(delegate
                {
                    I.ASilicon = value.ToString();
                }));
            }
        }

        public int AVerite
        {
            get
            {
                int tmp = 0;
                I.Invoke(new MethodInvoker(delegate
                {
                    tmp = int.Parse(I.AVerite ?? "0");
                }));
                return tmp;
            }
            set
            {
                I.Invoke(new MethodInvoker(delegate
                {
                    I.AVerite = value.ToString();
                }));
            }
        }

        public int AValorite
        {
            get
            {
                int tmp = 0;
                I.Invoke(new MethodInvoker(delegate
                {
                    tmp = int.Parse(I.AValorite ?? "0");
                }));
                return tmp;
            }
            set
            {
                I.Invoke(new MethodInvoker(delegate
                {
                    I.AValorite = value.ToString();
                }));
            }
        }

        public int AObsidian
        {
            get
            {
                int tmp = 0;
                I.Invoke(new MethodInvoker(delegate
                {
                    tmp = int.Parse(I.AObsidian ?? "0");
                }));
                return tmp;
            }
            set
            {
                I.Invoke(new MethodInvoker(delegate
                {
                    I.AObsidian = value.ToString();
                }));
            }
        }

        public int AAdamantium
        {
            get
            {
                int tmp = 0;
                I.Invoke(new MethodInvoker(delegate
                {
                    tmp = int.Parse(I.AAdamantium ?? "0");
                }));
                return tmp;
            }
            set
            {
                I.Invoke(new MethodInvoker(delegate
                {
                    I.AAdamantium = value.ToString();
                }));
            }
        }

        [XmlIgnore]
        public int TIron
        {
            get
            {
                int tmp = 0;
                I.Invoke(new MethodInvoker(delegate
                {
                    tmp = int.Parse(I.TIron ?? "0");
                }));
                return tmp;
            }
            set
            {
                I.Invoke(new MethodInvoker(delegate
                {
                    I.TIron = value.ToString();
                }));
            }
        }
        [XmlIgnore]
        public int TVerite
        {
            get
            {
                int tmp = 0;
                I.Invoke(new MethodInvoker(delegate
                {
                    tmp = int.Parse(I.TVerite ?? "0");
                }));
                return tmp;
            }
            set
            {
                I.Invoke(new MethodInvoker(delegate
                {
                    I.TVerite = value.ToString();
                }));
            }
        }
        [XmlIgnore]
        public int TValorite
        {
            get
            {
                int tmp = 0;
                I.Invoke(new MethodInvoker(delegate
                {
                    tmp = int.Parse(I.TValorite ?? "0");
                }));
                return tmp;
            }
            set
            {
                I.Invoke(new MethodInvoker(delegate
                {
                    I.TValorite = value.ToString();
                }));
            }
        }
        [XmlIgnore]
        public int TObsidian
        {
            get
            {
                int tmp = 0;
                I.Invoke(new MethodInvoker(delegate
                {
                    tmp = int.Parse(I.TObsidian ?? "0");
                }));
                return tmp;
            }
            set
            {
                I.Invoke(new MethodInvoker(delegate
                {
                    I.TObsidian = value.ToString();
                }));
            }
        }
        [XmlIgnore]
        public int TAdamantium
        {
            get
            {
                int tmp = 0;
                I.Invoke(new MethodInvoker(delegate
                {
                    tmp = int.Parse(I.TAdamantium ?? "0");
                }));
                return tmp;
            }
            set
            {
                I.Invoke(new MethodInvoker(delegate
                {
                    I.TAdamantium = value.ToString();
                }));
            }
        }
        [XmlIgnore]
        public int TSilicon
        {
            get
            {
                int tmp = 0;
                I.Invoke(new MethodInvoker(delegate
                {
                    tmp = int.Parse(I.TSilicon ?? "0");
                }));
                return tmp;
            }
            set
            {
                I.Invoke(new MethodInvoker(delegate
                {
                    I.TSilicon = value.ToString();
                }));
            }
        }


        public int MaxObs
        {
            get
            {
                int tmp=0;
                I.Invoke(new MethodInvoker(delegate
                {
                    tmp=(int) I.MaxObs;
                }));
                return tmp;
            }

            set
            {
                I.Invoke(new MethodInvoker(delegate
                {
                    I.MaxObs=value;
                }));
            }
        }

        public int MaxAda
        {
            get
            {
                int tmp = 0;
                I.Invoke(new MethodInvoker(delegate
                {
                    tmp = (int)I.MaxAda;
                }));
                return tmp;
            }

            set
            {
                I.Invoke(new MethodInvoker(delegate
                {
                    I.MaxAda = value;
                }));
            }
        }

        public bool DropCopper
        {
            get
            {
                bool tmp = false;
                I.Invoke(new MethodInvoker(delegate
                {
                    tmp = I.DropCopper;
                }));
                return tmp;
            }
            set
            {
                I.Invoke(new MethodInvoker(delegate
                {
                    I.DropCopper = value;
                }));
            }
        }

        public bool SkipCopper
        {
            get
            {
                bool tmp = false;
                I.Invoke(new MethodInvoker(delegate
                {
                    tmp = I.SkipCopper;
                }));
                return tmp;
            }
            set
            {
                I.Invoke(new MethodInvoker(delegate
                {
                    I.SkipCopper = value;
                }));
            }
        }

        public bool SkipIron
        {
            get
            {
                bool tmp = false;
                I.Invoke(new MethodInvoker(delegate
                {
                    tmp = I.SkipIron;
                }));
                return tmp;
            }
            set
            {
                I.Invoke(new MethodInvoker(delegate
                {
                    I.SkipIron = value;
                }));
            }
        }

        public bool SkipVerite
        {
            get
            {
                bool tmp = false;
                I.Invoke(new MethodInvoker(delegate
                {
                    tmp = I.SkipVerite;
                }));
                return tmp;
            }
            set
            {
                I.Invoke(new MethodInvoker(delegate
                {
                    I.SkipVerite = value;
                }));
            }
        }

        public bool SkipSilicon
        {
            get
            {
                bool tmp = false;
                I.Invoke(new MethodInvoker(delegate
                {
                    tmp = I.SkipSilicon;
                }));
                return tmp;
            }
            set
            {
                I.Invoke(new MethodInvoker(delegate
                {
                    I.SkipSilicon = value;
                }));
            }
        }

        public bool AutoRemoveRocks
        {
            get
            {
                bool tmp = false;
                I.Invoke(new MethodInvoker(delegate
                {
                    tmp = I.AutoRemoveRocks;
                }));
                return tmp;
            }
            set
            {
                I.Invoke(new MethodInvoker(delegate
                {
                    I.AutoRemoveRocks = value;
                }));
            }
        }

        public bool UseCrystal
        {
            get
            {
                bool tmp = false;
                I.Invoke(new MethodInvoker(delegate
                {
                    tmp = I.UseCrystal;
                }));
                return tmp;
            }
            set
            {
                I.Invoke(new MethodInvoker(delegate
                {
                    I.UseCrystal = value;
                }));
            }
        }

        public string FightSay
        {
            get
            {
                string tmp = "";
                I.Invoke(new MethodInvoker(delegate
                {
                    tmp = I.FightSay;
                }));
                return tmp;
            }
            set
            {
                I.Invoke(new MethodInvoker(delegate
                {
                    I.FightSay = value;
                }));
            }
        }

        [XmlIgnore]
        public int Mines_SelectedIndex
        {
            get
            {
                int tmp = 0;
                I.Invoke(new MethodInvoker(delegate
                {
                    tmp = I.Mines.SelectedIndex;
                }));
                return tmp;
            }
        }

        [XmlIgnore]
        public int WeightProgressbar
        {
            set
            {
                I.Invoke(new MethodInvoker(delegate
                {
                    I.WeightProgress.Maximum = World.Player.Strenght * 4 + 15;
                    I.WeightProgress.Value = value;
                }));
            }
        }

        public uint DoorLeft { get;  set; }
        public uint DoorRight { get;  set; }
        public ushort DoorLeftClosedGraphic { get;  set; }
        public ushort DoorRightClosedGraphic { get;  set; }
        public int HousePositionX { get; set; }
        public int HousePositionY { get; set; }
        public int RunePositionX { get;  set; }
        public int RunePositionY { get;  set; }
        public uint OreBox { get;  set; }
        public uint GemBox { get;  set; }
        public uint Weapon { get;  set; }
        public uint ResourceBox { get; set; }
    }
}
