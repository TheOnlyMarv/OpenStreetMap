using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStreetMap
{
    public class Vertex : IComparable<Vertex>
    {
        public long id;
        public double minDistance = Double.PositiveInfinity;
        public List<Edge> adjacencies;
        public Vertex previous;
        public double lat;
        public double lon;

        public Vertex(long id)
        {
            this.id = id;
            adjacencies = new List<Edge>();
        }
        public Vertex(OsmSharp.Node node) : this(node.Id ?? -1)
        {
            this.lat = node.Latitude ?? -1;
            this.lon = node.Longitude ?? -1;
        }
        override public string ToString()
        {
            return id.ToString();
        }

        public int CompareTo(Vertex other)
        {
            return minDistance.CompareTo(other.minDistance);
        }
    }
}
