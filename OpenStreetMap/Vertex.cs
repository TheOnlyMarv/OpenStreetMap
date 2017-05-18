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
        public long Id;
        public double MinDistance = Double.PositiveInfinity;
        public List<Edge> Adjacencies;
        public Vertex Previous;
        public double Latitude;
        public double Longitude;

        public Vertex(long id)
        {
            this.Id = id;
            Adjacencies = new List<Edge>();
        }
        public Vertex(OsmSharp.Node node) : this(node.Id ?? -1)
        {
            this.Latitude = node.Latitude ?? -1;
            this.Longitude = node.Longitude ?? -1;
        }
        override public string ToString()
        {
            return Id.ToString();
        }

        public int CompareTo(Vertex other)
        {
            return MinDistance.CompareTo(other.MinDistance);
        }
    }
}
