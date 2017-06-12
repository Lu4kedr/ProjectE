using System;

namespace Project_E.Lib.DrinkManager
{
    public class PotionLoseArgs : EventArgs
    {
        public Potion Potion { get; set; }

    }
}
