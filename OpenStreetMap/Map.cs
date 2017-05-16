using OsmSharp.Streams;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace OpenStreetMap
{
    public class Map
    {
        private Dictionary<long, Node> Nodes { get; set; }
        public Dictionary<long, Edge> Edges { get; set; }

        private Map()
        {
            this.Nodes = new Dictionary<long, Node>();
            this.Edges = new Dictionary<long, Edge>();
        }

        private Map(string path) : this()
        {
            using (var source = new FileInfo(path).OpenRead())
            {
                Console.Write("Parsing File... ");
                ParseFile(source);
                Console.WriteLine("COMPLETE");
            }
        }

        public static Map MapLoader(string path)
        {
            return new Map(path);
        }

        private void ParseFile(FileStream streamSource)
        {
            var source = new XmlOsmStreamSource(streamSource);
            foreach (var element in source)
            {
                if (element.Type == OsmSharp.OsmGeoType.Node)
                {
                    Node node = new Node((OsmSharp.Node)element);
                    this.Nodes.Add(node.Id, node);
                }
                else if (element.Type == OsmSharp.OsmGeoType.Way)
                {
                    string temp;
                    if (element.Tags != null && element.Tags.TryGetValue("highway", out temp))
                    {
                        AddEdges(CreateEdgesFromWay((OsmSharp.Way)element));
                    }
                }
            }
        }

        private List<Edge> CreateEdgesFromWay(OsmSharp.Way way)
        {
            List<Node> tempNodes = new List<Node>();
            foreach (var nodeId in way.Nodes)
            {
                Node node = Nodes[nodeId];
                if (node == null)
                {
                    throw new InvalidDataException("Node with Id: " + nodeId + " not found");
                }
                tempNodes.Add(node);
            }
            List<Edge> tempEdges = new List<Edge>();
            for (int i = 0; i < tempNodes.Count - 1; i++)
            {
                Node node1 = tempNodes[i];
                Node node2 = tempNodes[i + 1];
                tempEdges.Add(new Edge(way, node1, node2));
            }
            return tempEdges;
        }

        private void AddEdges(List<Edge> edges)
        {
            foreach (var edge in edges)
            {
                this.Edges.Add(edge.Id, edge);
            }
        }

        public void ExportToGraphiv(string path, int scaleMultiplier, string coordinatesPresentionFormat)
        {
            FileInfo fi = new FileInfo(path);
            using (var stream = new StreamWriter(fi.Create()))
            {
                stream.WriteLine("graph{");
                ExportNodesToGraphiv(stream, scaleMultiplier, coordinatesPresentionFormat);
                ExportEdgesToGraphiv(stream);
                stream.WriteLine("}");
            }
        }

        private void ExportEdgesToGraphiv(StreamWriter stream)
        {
            foreach (var edge in this.Edges)
            {
                string output = string.Format("{0} -- {1}", edge.Value.Node1.Id, edge.Value.Node2.Id);
                stream.WriteLine(output);
            }
        }

        private void ExportNodesToGraphiv(StreamWriter stream, int scaleMultiplier, string presentingFormat)
        {
            foreach (var node in this.Nodes)
            {
                string output = string.Format("{0} [pos=\"{1}, {2}!\", shape=\"point\"];",
                    node.Key,
                    (node.Value.Longitude * scaleMultiplier).ToString(presentingFormat, CultureInfo.InvariantCulture),
                    (node.Value.Latitude * scaleMultiplier).ToString(presentingFormat, CultureInfo.InvariantCulture));
                stream.WriteLine(output);
            }
        }


    }
}
