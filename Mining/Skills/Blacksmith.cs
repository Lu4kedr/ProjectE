using Phoenix;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mining.Skills
{
    public class Blacksmith
    {
        Check Check;

        [Command,BlockMultipleExecutions]
        public void BS()
        {
            try
            {
                UO.PrintInformation("Zamer Ore");
                UOItem or = new UOItem(UIManager.TargetObject());
                Graphic Ore = or.Graphic;
                UOColor Color = or.Color;

                UO.PrintInformation("Zamer bagl s ore");
                UOItem baackpack = new UOItem(UIManager.TargetObject());

                int tmp = baackpack.AllItems.First(x => x.Graphic == Ore && x.Color == Color).Amount;
                Check = new Check();
                Check.Start();
                Check.OnAfk += Check_OnAfk;
                while (tmp > 1)
                {
                    if (World.Player.Backpack.AllItems.First(x => x.Graphic == Ore && x.Color == Color)== null || (World.Player.Backpack.AllItems.First(x => x.Graphic == Ore && x.Color == Color).Amount < 11))
                    {
                        baackpack.AllItems.First(x => x.Graphic == Ore && x.Color == Color).Move(200, World.Player.Backpack);
                    }



                    foreach(var it in World.Player.Backpack.AllItems.Where(x=>x.Graphic== 0x0EED || x.Graphic== 0x1BF2).ToList())
                    {
                        if (it.Amount > 90) it.Move(ushort.MaxValue, baackpack);
                    }

                    tmp = baackpack.AllItems.First(x => x.Graphic == Ore && x.Color == Color).Amount;
                    UO.Wait(500);

                }
            }
            catch (Exception ex) { UO.PrintError(ex.Message); }
            finally
            {
                Check.OnAfk -= Check_OnAfk;
                Check.Stop();
            }
            

        }

        private void Check_OnAfk(object sender, EventArgs e)
        {
            Check.OnAfk -= Check_OnAfk;
            System.Media.SoundPlayer my_wave_file = new System.Media.SoundPlayer(Mine.Instance.AlarmPath);
            my_wave_file.Play();
            UO.Wait(200);
            Check.OnAfk += Check_OnAfk;
        }
    }
}
