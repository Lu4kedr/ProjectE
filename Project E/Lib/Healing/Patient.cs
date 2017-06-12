using Phoenix;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Project_E.Lib.Healing
{
    [Serializable]
    public class Patient
    {
        private uint serial;
        [XmlIgnore]
        public UOCharacter Character;


        public uint Serial
        {
            get
            {
                return serial;
            }
            set
            {
                serial = value;
                if (IsValid()) Character = new UOCharacter(serial);
                else UO.PrintError("Invalid serial");
            }
        }

        public int Equip
        { get; set; }


        public bool IsValid()
        {
            return Serial != default(uint);
        }

        public void Heal(string HealCmd)
        {
            UO.Say(HealCmd + Equip.ToString());
        }
    }
}
