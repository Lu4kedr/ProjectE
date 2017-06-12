using Phoenix;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Project_E.Lib.EquipSet
{
    [Serializable]
    public class EquipSet
    {
        [XmlArray]
        public List<EqSet> equipy { get; set; }

        public EquipSet()
        {
            equipy = new List<EqSet>();

        }

        public void Remove(int index)
        {
            if (index >= 0 && index >= equipy.Count) return;
            equipy.RemoveAt(index);

        }

        public void fillListBox(ListBox lb)
        {
            lb.Items.Clear();
            if (equipy.Count < 1 || equipy == null) return;
            foreach (EqSet e in equipy)
            {
                lb.Items.Add(e.SetName);
            }

        }

        public void Add()
        {
            UO.PrintInformation("Zmaer Bagl se setem");
            UOItem bag = new UOItem(UIManager.TargetObject());
            if (bag.Items.Count() > 0)
            {
                bag.Click();
                equipy.Add(new EqSet(bag));
            }
        }


    }
}
