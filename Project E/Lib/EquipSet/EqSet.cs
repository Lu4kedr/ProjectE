using Phoenix;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Project_E.Lib.EquipSet
{
    public delegate void Move(ushort amount, UOItem item);

    [Serializable]
    public class EqSet
    {
        private UOItem dropBagl;
        public string SetName { get; set; }
        [XmlArray]
        public List<uint> set { get; set; }
        private Dictionary<Serial, Layer> checkList = new Dictionary<Serial, Layer>();
        public EqSet()
        {

        }
        public EqSet(UOItem SetBAG)
        {
            set = new List<uint>() { SetBAG.Serial };
            SetBAG.Click();
            UO.Wait(100);
            SetBAG.Use();
            foreach (UOItem it in SetBAG.Items)
            {
                it.Click();
                set.Add(it.Serial);
            }
            SetName = SetBAG.Name;
        }
        public void Dress(UOItem dropBag)
        {
            dropBagl = dropBag;
            UOItem tmp;
            checkAndStore();
            foreach (Serial s in set)
            {
                if (s == set[0]) continue;
                tmp = new UOItem(s);
                if (tmp.Layer == Layer.LeftHand || tmp.Layer == Layer.RightHand)
                {
                    tmp.Equip();
                }
                else tmp.Use();
            }
            checkAndStore_();

        }
        public void DressOnly()
        {
            UOItem tmp;
            foreach (Serial s in set)
            {
                if (s == set[0]) continue;
                tmp = new UOItem(s);
                if (tmp.Layer == Layer.LeftHand || tmp.Layer == Layer.RightHand) tmp.Equip();
                else tmp.Use();
            }
        }

        public void checkAndStore(LayersCollection layers = null)
        {
            checkList.Clear();
            if (layers == null) layers = World.Player.Layers;
            foreach (UOItem it in layers)
            {
                if (checkList.ContainsKey(it.Serial) || it.Layer == Layer.None) continue;
                checkList.Add(it.Serial, it.Layer);
            }
        }
        public void checkAndStore_()
        {
            foreach (Serial s in checkList.Keys)
            {
                Move mo = new Move(Mov);
                mo.BeginInvoke(1, new UOItem(s), null, null);
                UO.Wait(50);
            }

        }

        public void Mov(ushort Amount, UOItem item)
        {
            item.Move(Amount, dropBagl);
        }

    }
}
