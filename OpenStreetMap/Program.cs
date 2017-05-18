using OsmSharp.Streams;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using OsmSharp;
using System.Globalization;

namespace OpenStreetMap
{
    class Program
    {
        static void Main(string[] args)
        {
            TestSomeDiffrentStuff(args);
            Console.ReadLine();
        }

        private static void TestSomeDiffrentStuff(string[] args)
        {
            Map map = Map.MapLoader(args[0]);
            List<Vertex> allVertices = map.Vertices;

            Vertex A = Dijkstra.FindNearestNearestNode(float.Parse(args[2]), float.Parse(args[3]), allVertices);
            Vertex Z = Dijkstra.FindNearestNearestNode(float.Parse(args[4]), float.Parse(args[5]), allVertices);

            Dijkstra.ComputePaths(A);
            Console.WriteLine("Distance to " + Z + ": " + Z.minDistance);
            List<Vertex> path = Dijkstra.GetShortestPathTo(Z);
            Console.Write("Path: ");
            foreach (var vertex in path)
            {
                Console.Write(vertex + " ");
            }
            ExportToGraphiv(args[1], 1000, "0.000000", A, Z, allVertices, path);
        }

        private static void ExportToGraphiv(string filepath, int scaleMultiplier, string formatString, Vertex start, Vertex end, List<Vertex> allVertices, List<Vertex> path)
        {
            FileInfo fi = new FileInfo(filepath);
            using (var stream = new StreamWriter(fi.Create()))
            {
                stream.WriteLine("graph{");
                ExportNodesToGraphiv(stream, scaleMultiplier, formatString, start, end, allVertices);
                ExportEdgesToGraphiv(stream, allVertices, path);
                stream.WriteLine("}");
            }
        }

        private static void ExportEdgesToGraphiv(StreamWriter stream, List<Vertex> allVertices, List<Vertex> path)
        {
            Dictionary<string, Tuple<Vertex, Vertex>> prepare = new Dictionary<string, Tuple<Vertex, Vertex>>();
            for (int i = 0; i < path.Count - 1; i++)
            {
                string s1 = path[i].id.ToString() + path[i + 1].id.ToString();
                string s2 = path[i + 1].id.ToString() + path[i].id.ToString();
                prepare.Add(s1, new Tuple<Vertex, Vertex>(path[i], path[i + 1]));
                prepare.Add(s2, null);
                string output = string.Format("{0} -- {1} [color=\"red\"];", path[i].id, path[i + 1].id);
                stream.WriteLine(output);
            }

            foreach (var vertex in allVertices)
            {
                foreach (var edge in vertex.adjacencies)
                {
                    string s1 = vertex.id.ToString() + edge.target.id.ToString();
                    string s2 = edge.target.id.ToString() + vertex.id.ToString();
                    try
                    {
                        prepare.Add(s1, new Tuple<Vertex, Vertex>(vertex, edge.target));
                        prepare.Add(s2, null);
                        allVertices.First(x => x.id == vertex.id);
                        allVertices.First(x => x.id == edge.target.id);
                        string output = string.Format("{0} -- {1};", vertex.id, edge.target.id);
                        stream.WriteLine(output);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        private static void ExportNodesToGraphiv(StreamWriter stream, int scaleMultiplier, string formatString, Vertex start, Vertex end, List<Vertex> allVertices)
        {
            foreach (var node in allVertices)
            {
                bool mark = node == start || node == end;
                string output = string.Format("{0} [pos=\"{1}, {2}!\", shape=\"point\" {3}];",
                    node.id,
                    (node.lon * scaleMultiplier).ToString(formatString, CultureInfo.InvariantCulture),
                    (node.lat * scaleMultiplier).ToString(formatString, CultureInfo.InvariantCulture),
                    mark ? "color=\"red\"" : ""
                    );
                stream.WriteLine(output);
            }
        }
    }
}
