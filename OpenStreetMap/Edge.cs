using System.Device.Location;

namespace OpenStreetMap
{
    public class Edge
    {
        public Vertex Target;
        public double Weight;
        public Edge(Vertex target, double weight)
        {
            this.Target = target;
            this.Weight = weight;
        }
    }
}
