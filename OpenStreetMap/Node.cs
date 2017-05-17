using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStreetMap
{
    public class Node
    {
        public long Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public IDictionary<long, Node> Neighbors { get; set; }
        public List<Node> OtherNeighbors { get; set; }

        public double Distance { get; set; }
        public bool Visited { get; set; }
        public List<double> Weights { get; set; }

        public Node(OsmSharp.Node node)
        {
            this.Id = node.Id ?? -1;
            this.Latitude = node.Latitude ?? -1;
            this.Longitude = node.Longitude ?? -1;
            this.Neighbors = new Dictionary<long, Node>();
            this.Visited = false;
            Weights = new List<double>();
            OtherNeighbors = new List<Node>();
        }

        public void SetNeighbor(List<Node> nodes)
        {
            foreach (Node node in nodes)
            {
                this.SetNeighbor(node);
            }
        }

        public void SetNeighbor(Node node)
        {
            try
            {
                this.Neighbors.Add(node.Id, node);
                this.OtherNeighbors.Add(node);
                this.SetWeight(node);
            }
            catch (ArgumentException)
            {
            }
        }

        private void SetWeight(Node node)
        {
            var sCoord = new GeoCoordinate(this.Latitude, this.Longitude);
            var eCoord = new GeoCoordinate(node.Latitude, node.Longitude);
            this.Weights.Add(Math.Abs(sCoord.GetDistanceTo(eCoord)));
        }
    }
}
