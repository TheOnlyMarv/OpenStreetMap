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
            ComputeEverything(args[0], args[1], float.Parse(args[2]), float.Parse(args[3]), float.Parse(args[4]), float.Parse(args[5]));
            Console.WriteLine("--- Everything is done ---");
            Console.ReadLine();
        }

        private static void ComputeEverything(string source, string destination, float startLat, float startLon, float endLat, float endLon)
        {
            Map map = Map.MapLoader(source);
            List<Vertex> allVertices = map.Vertices;

            Console.Write("Find nearest nodes (start, end)... ");
            Vertex A = Dijkstra.FindNearestNearestNode(startLat, startLon, allVertices);
            Vertex Z = Dijkstra.FindNearestNearestNode(endLat, endLon, allVertices);
            Console.WriteLine("COMPLETE");

            Console.Write("Calculate shortest path... ");
            Dijkstra.ComputePaths(A);
            Console.WriteLine("COMPLETE");

            Console.WriteLine("\n--- INFORMATION START ---");
            Console.WriteLine("Distance to " + Z + ": " + Z.MinDistance + " meter");
            List<Vertex> path = Dijkstra.GetShortestPathTo(Z);
            Console.Write("Path: ");
            foreach (var vertex in path)
            {
                Console.Write(vertex + " ");
            }
            Console.WriteLine("\n--- INFORMATION END ---\n");

            Console.Write("Export to Graphiv... ");
            ExportToGraphiv(destination, 1000, "0.000000", A, Z, allVertices, path);
            Console.WriteLine("COMPLETE");
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
                string s1 = path[i].Id.ToString() + path[i + 1].Id.ToString();
                string s2 = path[i + 1].Id.ToString() + path[i].Id.ToString();
                prepare.Add(s1, new Tuple<Vertex, Vertex>(path[i], path[i + 1]));
                prepare.Add(s2, null);
                string output = string.Format("{0} -- {1} [color=\"red\"];", path[i].Id, path[i + 1].Id);
                stream.WriteLine(output);
            }

            foreach (var vertex in allVertices)
            {
                foreach (var edge in vertex.Adjacencies)
                {
                    string s1 = vertex.Id.ToString() + edge.Target.Id.ToString();
                    string s2 = edge.Target.Id.ToString() + vertex.Id.ToString();
                    if (allVertices.FirstOrDefault(x => x.Id == vertex.Id) != null && allVertices.FirstOrDefault(x => x.Id == edge.Target.Id) != null)
                    {
                        if (prepare.Keys.FirstOrDefault(x => x == s1) == null && prepare.Keys.FirstOrDefault(x => x == s2) == null)
                        {
                            prepare.Add(s1, new Tuple<Vertex, Vertex>(vertex, edge.Target));
                            prepare.Add(s2, null);
                            string output = string.Format("{0} -- {1};", vertex.Id, edge.Target.Id);
                            stream.WriteLine(output);
                        }
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
                    node.Id,
                    (node.Longitude * scaleMultiplier).ToString(formatString, CultureInfo.InvariantCulture),
                    (node.Latitude * scaleMultiplier).ToString(formatString, CultureInfo.InvariantCulture),
                    mark ? "color=\"red\"" : ""
                    );
                stream.WriteLine(output);
            }
        }
    }
}
