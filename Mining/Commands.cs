using Phoenix;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mining
{
    public class Commands
    {
        public Commands()
        {

        }

        [Command("mine"), BlockMultipleExecutions]
        public void mine()
        {
            Mine.Instance.Start();
        }
        [Command]
        public void stop()
        {
            Mine.Instance.Stop();
        }

        [Command]
        public void vypis()
        {
            foreach(UOItem it in World.Player.Layers)
            {
                it.Click();
                UO.Wait(200);
                Notepad.WriteLine(it.Name);
            }
        }

    }
}
