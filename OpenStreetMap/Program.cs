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
            //Map map = Map.MapLoader(args[0]);
            //map.ExportToGraphiv(args[1], 1000, "0.000000");

            //Dijkstra dijkstra = new Dijkstra(map, float.Parse(args[2]), float.Parse(args[3]), float.Parse(args[4]), float.Parse(args[5]));
            //var result = dijkstra.GetPath();
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
                Console.Write(vertex + ", ");
            }
            ExportToGraphiv(args[1], 1000, "0.000000", A, Z, allVertices);
        }

        private static void ExportToGraphiv(string path, int scaleMultiplier, string formatString, Vertex start, Vertex end, List<Vertex> allVertices)
        {
            FileInfo fi = new FileInfo(path);
            using (var stream = new StreamWriter(fi.Create()))
            {
                stream.WriteLine("graph{");
                ExportNodesToGraphiv(stream, scaleMultiplier, formatString, start, end, allVertices);
                ExportEdgesToGraphiv(stream, allVertices);
                stream.WriteLine("}");
            }
        }

        private static void ExportEdgesToGraphiv(StreamWriter stream, List<Vertex> allVertices)
        {
            foreach (var vertex in allVertices)
            {
                foreach (var edge in vertex.adjacencies)
                {
                    string output = string.Format("{0} -- {1}", vertex.id, edge.target.id);
                    stream.WriteLine(output);
                }

            }
        }

        private static void ExportNodesToGraphiv(StreamWriter stream, int scaleMultiplier, string formatString, Vertex start, Vertex end, List<Vertex> allVertices)
        {
            foreach (var node in allVertices)
            {
                string output = string.Format("{0} [pos=\"{1}, {2}!\", shape=\"point\"];",//, shape=\"point\"
                    node.id,
                    (node.lon * scaleMultiplier).ToString(formatString, CultureInfo.InvariantCulture),
                    (node.lat * scaleMultiplier).ToString(formatString, CultureInfo.InvariantCulture));
                stream.WriteLine(output);
            }
        }
    }
}
