using Phoenix;
using Phoenix.WorldData;
using Project_E.Lib.WeaponsSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
