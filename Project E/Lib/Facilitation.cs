using Phoenix;
using Phoenix.WorldData;
using System.Collections.Generic;
using System.Linq;

namespace Project_E.Lib
{
    public class Facilitation
    {


        public void friend()
        {
            foreach (UOCharacter sum in World.Characters)
            {
                if (sum.Renamable || sum.Notoriety == Notoriety.Innocent)
                {
                    UO.Say("all friend");
                    UO.WaitTargetObject(sum.Serial);
                    UO.Wait(100);
                }
                UO.Wait(100);
            }
        }


        public void kill()
        {
            int wait = 100;
            foreach (UOCharacter enemy in World.Characters)
            {
                if (enemy.Renamable) continue;
                if (enemy.Distance > 11) continue;
                if (enemy.Notoriety == Notoriety.Enemy || enemy.Notoriety == Notoriety.Murderer)
                {
                    UO.Say("all kill");
                    UO.WaitTargetObject(enemy.Serial);
                    UO.Wait(wait = wait + 25);
                }
                UO.Wait(100);
            }
        }


        public void TakeAllFrom(Serial target, int delay)
        {
            foreach(var it in new UOItem(target).AllItems)
            {
                it.Move(ushort.MaxValue, Main.Instance.SGUI.LotBackpack);
                UO.Wait(delay);
            }
        }



        //public void sipky()
        //{
        //    World.Player.Backpack.Changed += World_ItemAdded;
        //    t = new System.Timers.Timer(300);
        //    t.Elapsed += T_Elapsed;
        //    t.Start();
        //}

        //private void T_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    List<Serial> tmp;
        //    UOItem temp;
        //    lock(lockBolt)
        //    {
        //        tmp = bolts.ToList();
        //    }
        //    foreach(var it in tmp)
        //    {
        //        temp = new UOItem(it);
        //        if(temp.Y>200 && temp.Distance<4)
        //        {
        //            temp.Move(ushort.MaxValue, UO.Backpack);
        //        }
        //        UO.Wait(100);
        //    }
        //}

        //public void sipky2()
        //{
        //    World.Player.Backpack.Changed -= World_ItemAdded;
        //    t.Stop();
        //    t = null;
        //}

        //System.Timers.Timer t;
        //List<Serial> bolts = new List<Serial>();
        //object lockBolt = new object();
        //private void World_ItemAdded(object sender, ObjectChangedEventArgs e)
        //{
        //    UOItem tmp = new UOItem(e.ItemSerial);

        //    if (tmp.Graphic == 0x1BFB)
        //    {
        //        if (tmp.X > 200 || tmp.Y > 200)
        //        {
        //            lock (lockBolt)
        //            {
        //                bolts.Add(tmp);
        //            }

        //        }
        //    }
            
        //}
    }
}
