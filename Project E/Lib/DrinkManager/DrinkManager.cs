using Phoenix;
using Phoenix.Communication.Packets;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace Project_E.Lib.DrinkManager
{
    public class DrinkManager
    {
        private static event EventHandler<PotionLoseArgs> OnPotionLose;
        Dictionary<UOColor, Potion> PotionCounter;
        Dictionary<string, int> PotionDelays;
        private int PotionsCount;
        //BackgroundWorker bw;
        DateTime DrinkTime=DateTime.Now;
        int LastDrinkDelay=0;
        System.Timers.Timer bw;
        bool Annouced = false;

        public DrinkManager()
        {
            PotionCounter = new Dictionary<UOColor, Potion>()
            {
                {0x0160, new Potion(){Name="Greater Heal", Amount=0, Command=".potionheal"}},
                {0x0059, new Potion(){Name="Spell Shield", Amount=0}},
                {0x00C5, new Potion(){Name="Greater Energy", Amount=0}},
                {0x0447, new Potion(){Name="Invisibility", Amount=0, Command=".potioninvis"}},
                {0x002B, new Potion(){Name="Cure", Amount=0, Command=".potioncure"}},
                {0x047D, new Potion(){Name="Great Cleverness", Amount=0}},
                {0x073E, new Potion(){Name="Cleverness", Amount=0, Command=".potionclever"}},
                {0x076B, new Potion(){Name="Greater Strength", Amount=0, Command=".potionstrength"}},
                {0x0388, new Potion(){Name="Strength", Amount=0, Command=".potionstrength"}},
                {0x0995, new Potion(){Name="Shrink", Amount=0}},
                {0x0985, new Potion(){Name="Reflection", Amount=0, Command=".potionreflex"}},
                {0x000F, new Potion(){Name="Mobility", Amount=0, Command=".potionmobility"}},
                {0x0993, new Potion(){Name="Dispell Explosion", Amount=0}},
                {0x00ED, new Potion(){Name="Greater Refresh", Amount=0, Command=".potionrefresh"}},
                {0x0027, new Potion(){Name="Refresh", Amount=0}},
                {0x0179, new Potion(){Name="Greater Poison", Amount=0} },
                {0x0980, new Potion(){Name="Nightsight", Amount=0, Command=".potionnightsight"} }
            

            };

            PotionDelays = new Dictionary<string, int>()
            {
                {".potioncure", 21 },
                {".potionheal", 21 },
                {".potioninvis", 21 },
                {".potionclever", 6 },
                {".potionrefresh", 21 },
                {".potionstrength", 6 },
                {".potionagility", 6 },// neni
                {".potionnightsight", 6 },
                {".potionstoneskin", 6 },
                {".potionmobility", 6 },
                {".potionreflex", 16 },

            };


            //bw = new BackgroundWorker();
            //bw.WorkerSupportsCancellation = true;
            //bw.DoWork += Bw_DoWork;
            //bw.RunWorkerAsync();
            //OnPotionLose += DrinkManager_OnPotionLose;

            bw = new System.Timers.Timer(500);
            bw.Elapsed += Bw_Elapsed;
            bw.Start();
            Core.RegisterClientMessageCallback(0xAD, OnPotionRequest);
        }

        private void Bw_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (DateTime.Now - DrinkTime > TimeSpan.FromSeconds(LastDrinkDelay))
            {
                if (!Annouced)
                {
                    UO.PrintInformation("Muzes Pit!");
                    World.Player.Print("Muzes Pit!");
                    Annouced = true;
                }
            }
            else Annouced = false;
            PotionsCount = CountPotion();
        }

        private CallbackResult OnPotionRequest(byte[] data, CallbackResult prevState)
        {
            UnicodeSpeechRequest a = new UnicodeSpeechRequest(data);
            if(PotionDelays.ContainsKey(a.Text))
            {
                foreach(var it in PotionCounter.Values.Where(x=>x.Command.Equals(a.Text)).ToList())
                {
                    if(it.Amount==0) return CallbackResult.Eat; 
                }
                if (DateTime.Now-DrinkTime>TimeSpan.FromSeconds(LastDrinkDelay))
                {
                    DrinkTime = DateTime.Now;
                    LastDrinkDelay = PotionDelays[a.Text];
                    return CallbackResult.Normal;
                }
                else
                {
                    UO.PrintError("Jeste nemuzes pit!!");
                    return CallbackResult.Eat;
                }
            }
            return CallbackResult.Normal;
        }

        private void DrinkManager_OnPotionLose(object sender, PotionLoseArgs e)
        {
            if (e.Potion.Amount > 0)
                UO.PrintError("Zbyva  {0}  {1}", e.Potion.Amount, e.Potion.Name);
            else
                UO.PrintError("Dosel potion {0}", e.Potion.Name);
        }

        //private void Bw_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    bool Annouced = false;
        //    while(!bw.CancellationPending)
        //    {
        //        if (DateTime.Now - DrinkTime > TimeSpan.FromSeconds(LastDrinkDelay))
        //        {
        //            if (!Annouced)
        //            {
        //                UO.PrintInformation("Muzes Pit!");
        //                World.Player.Print("Muzes Pit!");
        //                Annouced = true;
        //            }
        //        }
        //        else Annouced = false;
        //        Thread.Sleep(500);
        //        PotionsCount = CountPotion();
        //    }
        //}

        
        private int CountPotion()
        {
            int tmp;
            int PotionsCount = 0;
            foreach (UOItem it in World.Player.Backpack.AllItems.Where(x => x.Graphic == 0x0F0E && PotionCounter.ContainsKey(x.Color)).ToList()) 
            {
                tmp = PotionCounter[it.Color].Amount;
                PotionCounter[it.Color] = new Potion() {Name= PotionCounter[it.Color].Name, Amount=it.Amount };
                PotionsCount += it.Amount;
                if(it.Amount<tmp)
                {
                    EventHandler<PotionLoseArgs> temp = OnPotionLose;
                    if(temp!=null)
                    {
                        foreach(EventHandler<PotionLoseArgs> ev in OnPotionLose.GetInvocationList())
                        {
                            ev.BeginInvoke(this, new PotionLoseArgs()
                            {
                                Potion = new Potion() { Amount = it.Amount, Name = PotionCounter[it.Color].Name }
                            }, null, null);
                        }
                    }
                }
                Thread.Sleep(50);
            }
            return PotionsCount;
        }


    }
}
