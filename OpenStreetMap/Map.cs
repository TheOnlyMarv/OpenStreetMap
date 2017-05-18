using OsmSharp.Streams;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
using System.IO;
using System.Linq;

namespace OpenStreetMap
{
    public class Map
    {


        public List<Vertex> Vertices { get; set; }

        private Map()
        {
            Vertices = new List<Vertex>();
        }

        private Map(string path) : this()
        {
            using (var source = new FileInfo(path).OpenRead())
            {
                Console.Write("Parsing File... ");
                LoadOSMFile(source);
                Console.WriteLine("COMPLETE");
            }
        }

        public static Map MapLoader(string path)
        {
            return new Map(path);
        }

        private void LoadOSMFile(Stream fileSource)
        {
            var source = new XmlOsmStreamSource(fileSource);
            foreach (var element in source)
            {
                if (element.Type == OsmSharp.OsmGeoType.Node)
                {
                    Vertex vertex = new Vertex((OsmSharp.Node)element);
                    Vertices.Add(vertex);
                }
                else if (element.Type == OsmSharp.OsmGeoType.Way)
                {
                    string temp;
                    if (element.Tags != null && element.Tags.TryGetValue("highway", out temp))
                    {
                        CreateEdges((OsmSharp.Way)element, Vertices);
                    }
                }
            }
            Vertices = tempVertices.Values.ToList();
        }

        static Dictionary<long, Vertex> tempVertices = new Dictionary<long, Vertex>();

        private void CreateEdges(OsmSharp.Way element, List<Vertex> allVertices)
        {
            if (element.Nodes.Length > 1)
            {
                for (int i = 0; i < element.Nodes.Length - 1; i++)
                {
                    Vertex v1 = allVertices.Find(x => x.id == element.Nodes[i]);
                    Vertex v2 = allVertices.Find(x => x.id == element.Nodes[i + 1]);
                    v1.adjacencies.Add(new Edge(v2, calculateDistance(v1, v2)));
                    v2.adjacencies.Add(new Edge(v1, calculateDistance(v2, v1))); //evtl problematisch => loop
                    try
                    {
                        tempVertices.Add(v1.id, v1);
                        tempVertices.Add(v2.id, v2);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        static Func<Vertex, Vertex, double> calculateDistance = (v1, v2) =>
        {
            var sCoord = new GeoCoordinate(v1.lat, v1.lon);
            var eCoord = new GeoCoordinate(v2.lat, v2.lon);
            return Math.Abs(sCoord.GetDistanceTo(eCoord));
        };

        //    public void ExportToGraphiv(string path, int scaleMultiplier, string coordinatesPresentionFormat)
        //    {
        //        FileInfo fi = new FileInfo(path);
        //        using (var stream = new StreamWriter(fi.Create()))
        //        {
        //            stream.WriteLine("graph{");
        //            ExportNodesToGraphiv(stream, scaleMultiplier, coordinatesPresentionFormat);
        //            ExportEdgesToGraphiv(stream);
        //            stream.WriteLine("}");
        //        }
        //    }

        //    private void ExportEdgesToGraphiv(StreamWriter stream)
        //    {
        //        foreach (var edge in this.Edges)
        //        {
        //            string output = string.Format("{0} -- {1}", edge.Value.Node1.Id, edge.Value.Node2.Id);
        //            stream.WriteLine(output);
        //        }
        //    }

        //    private void ExportNodesToGraphiv(StreamWriter stream, int scaleMultiplier, string presentingFormat)
        //    {
        //        foreach (var node in this.Nodes)
        //        {
        //            string output = string.Format("{0} [pos=\"{1}, {2}!\", shape=\"point\"];",
        //                node.Key,
        //                (node.Value.Longitude * scaleMultiplier).ToString(presentingFormat, CultureInfo.InvariantCulture),
        //                (node.Value.Latitude * scaleMultiplier).ToString(presentingFormat, CultureInfo.InvariantCulture));
        //            stream.WriteLine(output);
        //        }
        //    }

    }
}
