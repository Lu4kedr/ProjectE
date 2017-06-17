using Phoenix;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Mining
{
    [Serializable]
    
    public class Map
    {

        private List<MineField> fields;
        public int Index { get; set; }
        public string Name { get; set; }

        [XmlArray]
        public List<MineField> Fields
        {
            get
            {
                if (fields == null) fields = new List<MineField>();
                return fields;
            }

            set
            {
                fields = value;
            }
        }

        // zavaly
        private readonly List<Graphic> Obsatcles = new List<Graphic>() { 0x1363, 0x1364, 0x1365, 0x1366, 0x1367, 0x1368, 0x1369, 0x136A, 0x136B, 0x136C, 0x136D };



        public void FindObstacles()
        {
            World.FindDistance = 15;
            foreach (UOItem it in World.Ground.Where(x => Obsatcles.Any(a => x.Graphic.Equals(a))))
            {
                MineField tmp = Fields.Find(x => x.Location.X == it.X && x.Location.Y == it.Y);
                if (tmp == null) continue;
               // UO.PrintInformation("Zaval zaregistrovan"); 
                if (Fields[Fields.IndexOf(tmp)].State != MineFieldState.Obstacle)
                    Fields[Fields.IndexOf(tmp)].State = MineFieldState.Obstacle;
            }
            // Set Fields empty when someone stands on them
            foreach(var it in World.Characters)
            {
                MineField tmp = Fields.Find(x => x.Location.X == it.X && x.Location.Y == it.Y);
                if (tmp == null) continue;
                Fields[Fields.IndexOf(tmp)].State = MineFieldState.Empty;
            }
        }

        public void RemoveNearObstacles(Action<UOItem,int> RemoveObstacle)
        {
            World.FindDistance = 2;
            foreach (UOItem it in World.Ground.Where(x => Obsatcles.Any(a => x.Graphic.Equals(a)) && x.Distance<3))
            {
                it.Click();
                UO.Wait(300);
                if (it.Name.Contains("dulni zaval"))
                    RemoveObstacle(it,0);
            }
        }

        public void Record(string Name, int index)
        {
            UO.PrintInformation("Projdi vsechna pole, po kterych lze chodit/kopat");
            UO.PrintWarning("STOP pro ukonceni");
            UO.PrintWarning("WALK pro prochazeni NEtezebnich poli");
            UO.PrintWarning("MINE pro prochazeni tezebnich poli");
            bool RecordState = false;
            int X = World.Player.X;
            int Y = World.Player.Y;
            var RecordRun = true;
            this.Name = Name;
            this.Index = index;

            Journal.ClearAll();
            while (RecordRun)
            {
                if (X != World.Player.X || Y != World.Player.Y)
                {
                    X = World.Player.X;
                    Y = World.Player.Y;
                }
                if (Journal.Contains("WALK"))
                {
                    UO.PrintInformation("Nasledujici pole budou oznaceny jen pro CHUZI");
                    Journal.Clear();
                    RecordState = false;

                }
                if (Journal.Contains("MINE"))
                {
                    UO.PrintInformation("Nasledujici pole budou oznaceny pro TEZBU");
                    Journal.Clear();
                    RecordState = true;
                }
                if (Journal.Contains("STOP"))
                    RecordRun = false;
                if (this.Fields.Contains(new MineField() { Location = new System.Drawing.Point(X, Y) })) continue;
                if (RecordState)//mine
                {
                    this.Fields.Add(new MineField()
                    {
                        IsExploitable = true,
                        //IsObstacle = false,
                        IsWalkable = true,
                        Location = new System.Drawing.Point(X, Y),
                        MapIndex = index,
                        State = MineFieldState.Unknown
                    });
                }
                else// only walk
                {
                    this.Fields.Add(new MineField()
                    {
                        IsExploitable = false,
                        //IsObstacle = false,
                        IsWalkable = true,
                        Location = new System.Drawing.Point(X, Y),
                        MapIndex = index,
                        State = MineFieldState.Walkable
                    });
                }
                UO.Wait(100);
            }


            UO.PrintInformation(".Record Done");

        }


    }
}
