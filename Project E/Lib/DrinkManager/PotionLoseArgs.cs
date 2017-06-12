using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_E.Lib.DrinkManager
{
    public class PotionLoseArgs : EventArgs
    {
        public Potion Potion { get; set; }

    }
}
