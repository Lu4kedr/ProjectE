using Phoenix;
using System;

namespace Mining
{
    public class OnOreAddedArgs :EventArgs
    {
        public string Type { get; set; }
    }
}