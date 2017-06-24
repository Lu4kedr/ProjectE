using Phoenix;
using Phoenix.WorldData;
using System.Collections.Generic;

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



        public void sipky()
        {
            World.Player.Backpack.Changed += World_ItemAdded;
        }


        List<Serial> bolts = new List<Serial>();
        private void World_ItemAdded(object sender, ObjectChangedEventArgs e)
        {
            UOItem tmp = new UOItem(e.ItemSerial);

                if(tmp.Graphic== 0x1BFB)// && (tmp.X>200 || tmp.Y>200))
                {
                   if(tmp.Container!=World.Player.Backpack) bolts.Add(tmp);
                    tmp.Click();
                UO.Print(e.Type);
                }
            
        }
    }
}
