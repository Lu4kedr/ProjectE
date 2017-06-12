using System;

namespace Project_E.GUI
{
    public class OnChangedArgs : EventArgs
    {
        public string Name { get; set; }
        public int TabControlSelectedID { get; set; }
    }
}
