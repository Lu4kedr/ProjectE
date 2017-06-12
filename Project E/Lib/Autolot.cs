using Phoenix;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace Project_E.Lib
{
    public class Autolot : IDisposable
    {
        private bool IsDisposed = false;
        private List<Graphic> toCarv = new List<Graphic> { 0x2006, 0x0EE3, 0x0EE4, 0x0EE5, 0x0EE6, 0x2006 };//pvuciny+mrtvola
        private readonly List<string> jezdidla = new List<string> { "body of mustang", "body of zostrich", "body of oclock", "body of orn", "oody of ledni medved", "body of ridgeback", "body of ridgeback savage" };
        private ushort[] FOOD = { 0x0978, 0x097A, 0x097B, 0x097E, 0x098C, 0x0994, 0x09B7, 0x09B9, 0x09C0, 0x09C9, 0x09D0, 0x09D1, 0x09D2, 0x09E9, 0x09EA, 0x09EC, 0x09F2, 0x0C5C, 0x0C64, 0x0C66, 0x0C6A, 0x0C6D, 0x0C70, 0x0C72, 0x0C74, 0x0C77, 0x0C79, 0x0C7B, 0x0C7F, 0x0D39, 0x103B, 0x1040, 0x1041, 0x1608, 0x1609, 0x160A, 0x171F, 0x1726, 0x1727, 0x1728, 0x172A };
        private Dictionary<Graphic, int> LotItems = new Dictionary<Graphic, int>() { { 0x0eed, 0 }, { 0x14EB, 0 }, { 0x0E76, 0 } };
        private BackgroundWorker bw;
        private bool food, gems, regeants, feathers, bolts, leather, extend1, extend2;

        // Local property - add & delete items from LotItems
        private bool _Food
        {
            get { return food; }
            set
            {
                try
                {
                    food = value;
                    if (value)
                    {
                        foreach (ushort r in FOOD)
                        {
                            LotItems.Add(r, 0);
                        }
                    }
                    else
                    {
                        foreach (ushort r in FOOD)
                        {
                            LotItems.Remove(r);
                        }
                    }
                }
                catch (Exception ex) { UO.PrintError(ex.Message); }
            }
        }
        private bool _Gems
        {
            get { return gems; }
            set
            {
                try
                {
                    gems = value;
                    if (value)
                    {
                        for (ushort i = 0x0F0F; i < 0x0F31; i++)
                        {
                            LotItems.Add(i, 0);
                        }

                    }
                    else
                    {
                        for (ushort i = 0x0F0F; i < 0x0F31; i++)
                        {
                            LotItems.Remove(i);
                        }
                    }
                }
                catch (Exception ex) { UO.PrintError(ex.Message); }
            }
        }
        private bool _Regeants
        {
            get { return regeants; }
            set
            {
                try
                {
                    regeants = value;
                    if (value)
                    {
                        for (ushort i = 0x0F78; i < 0x0F92; i++)
                        {
                            LotItems.Add(i, 0);
                        }

                    }
                    else
                    {
                        for (ushort i = 0x0F78; i < 0x0F92; i++)
                        {
                            LotItems.Remove(i);
                        }
                    }
                }
                catch (Exception ex) { UO.PrintError(ex.Message); }
            }
        }
        private bool _Feathers
        {
            get { return feathers; }
            set
            {
                feathers = value;
                try
                {
                    if (value)
                    {
                        LotItems.Add(0x1BD1, 0);
                    }
                    else
                    {
                        LotItems.Remove(0x1BD1);

                    }
                }
                catch (Exception ex) { UO.PrintError(ex.Message); }
            }
        }
        private bool _Bolts
        {
            get { return bolts; }
            set
            {
                try
                {
                    bolts = value;
                    if (value)
                    {
                        LotItems.Add(0x1BFB, 0);
                        LotItems.Add(0x0F3F, 0);
                    }
                    else
                    {
                        LotItems.Remove(0x1BFB);
                        LotItems.Remove(0x0F3F);

                    }
                }
                catch (Exception ex) { UO.PrintError(ex.Message); }
            }
        }
        private bool _Leather
        {
            get { return leather; }
            set
            {
                leather = value;
                try
                {
                    if (value)
                    {
                        LotItems.Add(0x1078, 0);
                    }
                    else
                    {
                        LotItems.Remove(0x1078);

                    }
                }
                catch (Exception ex) { UO.PrintError(ex.Message); }
            }
        }
        private bool _Extend1
        {
            get { return extend1; }
            set
            {
                try
                {
                    extend1 = value;
                    if (value)
                    {
                        LotItems.Add(Main.Instance.SGUI.Extend1, 0);
                    }
                    else
                    {
                        LotItems.Remove(Main.Instance.SGUI.Extend1);

                    }
                }
                catch (Exception ex) { UO.PrintError(ex.Message); }
            }
        }
        private bool _Extend2
        {
            get { return extend2; }
            set
            {
                try
                {
                    extend2 = value;
                    if (value)
                    {
                        LotItems.Add(Main.Instance.SGUI.Extend2, 0);
                    }
                    else
                    {
                        LotItems.Remove(Main.Instance.SGUI.Extend2);

                    }
                }
                catch (Exception ex) { UO.PrintError(ex.Message); }
            }
        }


        // Getters from Settings
        private bool Food
        {
            get
            {
                return Main.Instance.SGUI.Food;
            }

        }

        private bool Gems
        {
            get
            {
                return Main.Instance.SGUI.Gems;
            }

        }

        private bool Regeants
        {
            get
            {
                return Main.Instance.SGUI.Regeants;
            }

        }

        private bool Feathers
        {
            get
            {
                return Main.Instance.SGUI.Feathers;
            }

        }

        private bool Bolts
        {
            get
            {
                return Main.Instance.SGUI.Bolts;
            }

        }

        private bool Leather
        {
            get
            {
                return Main.Instance.SGUI.Leather;
            }

        }

        private bool Extend1
        {
            get
            {
                return Main.Instance.SGUI.Extend1 == 0 ? false : true;
            }

        }

        private bool Extend2
        {
            get
            {
                return Main.Instance.SGUI.Extend2 == 0 ? false : true;
            }

        }



        public Autolot()
        {
            _Food = Food;
            _Gems = Gems;
            _Regeants = Regeants;
            _Feathers = Feathers;
            _Bolts = Bolts;
            _Leather = Leather;
            _Extend1 = Extend1;
            _Extend2 = Extend2;
            Main.Instance.EreborGUI.OnChanged += Erebor_OnChanged;
        }

        private void Erebor_OnChanged(object sender, EventArgs e)
        {
            if (_Food != Food) _Food = Food;
            if (_Gems != Gems) _Gems = Gems;
            if (_Regeants != Regeants) _Regeants = Regeants;
            if (_Feathers != Feathers) _Feathers = Feathers;
            if (_Bolts != Bolts) _Bolts = Bolts;
            if (_Leather != Leather) _Leather = Leather;
            if (_Extend1 != Extend1) _Extend1 = Extend1;
            if (_Extend2 != Extend2) _Extend2 = Extend2;

        }

        public void Start()
        {
            bw = new BackgroundWorker();
            bw.DoWork += Bw_DoWork;
            bw.WorkerSupportsCancellation = true;
            bw.RunWorkerAsync();
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!((BackgroundWorker)sender).CancellationPending)
            {
                if (Main.Instance.EreborGUI.LotOnOff.BackColor == System.Drawing.Color.Green) Checker();
                Thread.Sleep(300);
            }
        }

        private void Checker()
        {
            World.FindDistance = 4;

            // Check Gold & mesec
                if (World.Player.Backpack.AllItems.FindType(0x0EED).Amount >= ushort.Parse(Main.Instance.SGUI.GoldLimit))
                {
                    UO.Say(".mesec");
                }

            foreach (UOItem it in World.Ground.Where(x => x.Graphic == 0x2006 && x.Items.CountItems() < 7).ToList())
            {
                Lot(it);
            }
        }

        private void Lot(UOItem it)
        {
            foreach (UOItem i in it.Items.Where(item => LotItems.Any(li => item.Graphic == li.Key)).ToList())//it.AllItems)
            {
                UO.MoveItem(i, ushort.MaxValue, Main.Instance.SGUI.LotBackpack == default(uint) ? World.Player.Backpack : Main.Instance.SGUI.LotBackpack);
                UO.Wait(300);
            }
        }


        public void Carving()
        {
            World.FindDistance = 4;
            foreach (UOItem it in World.Ground.Where(x => x.Distance < 4 && toCarv.Any(p => x.Graphic == p)))//x.Graphic == 0x2006))// && jezdidla.All(y => y != x.Name.ToLower()) ))//!jezdidla.Contains(x.Name)).ToList())
            {
                it.Click();
                UO.Wait(100);
                if (jezdidla.Contains(it.Name.ToLower())) continue;

                it.WaitTarget();
                new UOItem(Main.Instance.SGUI.CarvTool).Use();
                UO.Wait(300);
            }

        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Autolot()
        {
            if (!IsDisposed)
            {
                Dispose(false);
            }
        }

        private void Dispose(bool Disposing)
        {
            if (Disposing)
            {
                bw.CancelAsync();
                bw.DoWork -= Bw_DoWork;
                bw = null;
                Main.Instance.EreborGUI.OnChanged -= Erebor_OnChanged;
            }
        }




    }
}
