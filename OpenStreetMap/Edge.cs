using System.Device.Location;

namespace OpenStreetMap
{
    public class Edge
    {
        public long Id { get; set; }
        public long WayId { get; set; }
        public string StreetName { get; set; }
        public int MaxSpeed { get; set; }
        public Node Node1 { get; set; }
        public Node Node2 { get; set; }
        public bool OneWay { get; set; }
        public double Distance { get; set; }
        public OsmSharp.Way OriginalWay { get; set; }

        public static long IdCounter = 0;

        private Edge()
        {
            // Default setting
            Id = IdCounter++;
            WayId = -1;
            StreetName = "undefine";
            MaxSpeed = 50;
            OneWay = false;
            Distance = -1;
        }

        public Edge(OsmSharp.Way way, Node node1, Node node2) : this()
        {
            if (way.Id != null)
            {
                WayId = (long)way.Id;
            }

            if (way.Tags != null)
            {
                // Getting speed information
                string tempString;
                way.Tags.TryGetValue("maxspeed", out tempString);
                int maxSpeed = this.MaxSpeed;
                int.TryParse(tempString ?? "50", out maxSpeed);
                this.MaxSpeed = maxSpeed;

                // Getting direction information
                way.Tags.TryGetValue("oneway", out tempString);
                if (tempString != null && tempString == "yes")
                {
                    this.OneWay = true;
                }

                // Getting streetname information
                way.Tags.TryGetValue("name", out tempString);
                this.StreetName = tempString ?? "undefine";
            }

            this.Node1 = node1;
            this.Node2 = node2;

            // Save original way
            this.OriginalWay = way;

            // Calculate distance
            var sCoord = new GeoCoordinate(this.Node1.Latitude, this.Node1.Longitude);
            var eCoord = new GeoCoordinate(this.Node2.Latitude, this.Node2.Longitude);
            this.Distance = sCoord.GetDistanceTo(eCoord);
        }
    }
}
