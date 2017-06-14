using Phoenix.WorldData;
using System;

namespace Mining
{
    public class EnemyAppearedArgs : EventArgs
    {
        public UOCharacter Enemy { get; set; }
        public bool CK { get; set; }
        public bool TopMonster { get; set; }
    }
}