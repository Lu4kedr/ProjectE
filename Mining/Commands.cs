using Phoenix;
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
    }
}
