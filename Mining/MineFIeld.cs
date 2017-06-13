using Phoenix.WorldData;
using System;
using System.Drawing;

namespace Mining
{
    [Serializable]
    public class MineField : IEquatable<MineField>, IComparable<MineField>
    {
        private bool isWalkable = false;
        private bool isExploitable = false;
        private bool isObstacle = false;
        private MineFieldState state = MineFieldState.Empty;


        public Point Location { get; set; }

        public DateTime TimeStamp { get; set; }

        public MineFieldState State
        {
            get
            {
                if (state == MineFieldState.Empty)
                {
                    if (DateTime.Now - TimeStamp > TimeSpan.FromMinutes(20))
                    {
                        state = MineFieldState.Unknown;
                        IsExploitable = true;
                    }

                }
                if (state == MineFieldState.Obstacle)
                {
                    if (DateTime.Now - TimeStamp > TimeSpan.FromMinutes(5))
                    {
                        state = MineFieldState.Unknown;
                        IsExploitable = true;
                    }

                }
                return state;
            }
            set
            {
                if (value == MineFieldState.Empty)
                {
                    TimeStamp = DateTime.Now;
                    IsExploitable = false;
                }
                if (value == MineFieldState.Obstacle)
                {
                    TimeStamp = DateTime.Now;
                    IsExploitable = false;
                }
                state = value;
            }
        }

        public int MapIndex { get; set; }

        public bool IsWalkable
        {
            get
            {
                return isWalkable;
            }
            set
            {
                isWalkable = value;
            }
        }

        private char IsWalkableXML
        {
            get
            {
                return isWalkable == true ? 'Y' : 'N';
            }
            set
            {
                if (value == 'Y')
                    IsWalkable = true;
                else
                    IsWalkable = false;
            }
        }

        public bool IsExploitable
        {
            get
            {
                MineFieldState s= State;
                return isExploitable;
            }
            set
            {
                isExploitable = value;
            }
        }

        private char IsExploitableXML
        {
            get
            {
                return isExploitable == true ? 'Y' : 'N';
            }
            set
            {
                if (value == 'Y')
                    IsExploitable = true;
                else
                    IsExploitable = false;
            }
        }


        private char IsObstacleXML
        {
            get
            {
                return isObstacle == true ? 'Y' : 'N';
            }
            set
            {
                if (value == 'Y')
                    isObstacle = true;
                else
                    isObstacle = false;
            }
        }

        public double Distance
        {
            get
            {
                return Math.Sqrt(Math.Pow((Location.X - (World.Player.X)), 2) + Math.Pow((Location.Y - (World.Player.Y)), 2));
            }
        }

        public bool Equals(MineField other)
        {
            if (other == null) return false;
            return Location.Equals(other.Location);
        }

        public int CompareTo(MineField other)
        {
            if (other == null) return 1;
            if (Distance < other.Distance) return -1;
            if (Distance > other.Distance) return 1;
            if (Distance == other.Distance)
            {
                if (other.TimeStamp == default(DateTime)) return -1;
                if (TimeStamp < other.TimeStamp) return 1;
                if (TimeStamp > other.TimeStamp) return -1;
                return 0;
            }
            return 1;
        }
    }
}
