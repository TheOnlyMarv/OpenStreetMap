using System;
using System.Collections.Generic;
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

        public Node(OsmSharp.Node node)
        {
            this.Id = node.Id ?? -1;
            this.Latitude = node.Latitude ?? -1;
            this.Longitude = node.Longitude ?? -1;
        }
    }
}
