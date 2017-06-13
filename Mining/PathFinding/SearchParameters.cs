using System.Drawing;


namespace Mining.PathFinding
{
    /// <summary>
    /// Defines the parameters which will be used to find a path across a section of the map
    /// </summary>
    public class SearchParameters
    {
        public Point StartLocation { get; set; }

        public Point EndLocation { get; set; }
        
        public Map Map { get; set; }

        public SearchParameters(Point startLocation, Point endLocation, Map map)
        {
            this.StartLocation = startLocation;
            this.EndLocation = endLocation;
            this.Map = map;
        }
    }
}
