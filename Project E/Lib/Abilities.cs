using Phoenix;
using Phoenix.WorldData;
using System;

namespace Project_E.Lib
{
    public class Abilities
    {

        public void probo(DateTime HiddenTime, Action hidoff)
        {
            UOCharacter target = new UOCharacter(Aliases.GetObject("laststatus"));
            bool first = true;
            while (World.Player.Hidden)
            {

                UO.Wait(100);
                if (!target.Serial.Equals(Aliases.GetObject("laststatus")))
                    target = new UOCharacter(Aliases.GetObject("laststatus"));
                if (DateTime.Now - HiddenTime < TimeSpan.FromSeconds(3)) continue;
                if (first)
                {
                    UO.PrintError("Muzes Bodat!");
                    first = false;
                }
                if (target.Distance < 2)
                {
                    target.WaitTarget();
                    UO.Say(".usehand");
                    UO.Wait(200);
                }

            }
            hidoff();
            UO.Say(",hid");

        }

        public void kudla(WeaponsSet.Weapons weapons)
        {
            UOCharacter cil = new UOCharacter(Aliases.GetObject("laststatus"));
            if (cil.Distance > 6)
            {
                UO.PrintError("Moc daleko");
                return;
            }
            UO.Say(".throw");
            weapons.ActualWeapon.Equip();
            if (Journal.WaitForText(true, 1000, "Nemas zadny cil.", "Nevidis na cil"))
            {
                UO.PrintInformation("HAZ!!");
                return;
            }
            UO.Wait(1000);
            UO.PrintInformation("3");
            UO.Wait(1000);
            UO.PrintInformation("2");
            UO.Wait(1000);
            UO.PrintInformation("1");
            UO.Wait(1000);
            UO.PrintInformation("HAZ!!");
            World.Player.Print("Hazej!!!");

        }





        public void Sacrafire(Action bandage)
        {
            if (World.Player.Mana == World.Player.MaxMana)
            {
                UO.PrintInformation("Mas plnou manu!!");
                if (World.Player.Hits < World.Player.MaxHits) bandage();
                return;
            }
            if (World.Player.Hits > 80)
            {
                if (World.Player.Hits < World.Player.MaxHits)
                {
                    bandage();
                    UO.Say(".voodooobet");
                }
                else
                {
                    UO.Say(".voodooobet");
                    UO.Wait(100);
                    bandage();
                }
                UO.Wait(100);
            }
            else UO.PrintWarning("malo HP!!");
        }

    }
}
