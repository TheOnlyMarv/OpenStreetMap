using System.Device.Location;

namespace OpenStreetMap
{
    public class Edge
    {
        public Vertex target;
        public double weight;
        public Edge(Vertex target, double weight)
        {
            this.target = target;
            this.weight = weight;
        }
    }
}
