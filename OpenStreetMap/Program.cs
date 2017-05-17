using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using TestOSM;

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
            List<Vertex> allVertices = new List<Vertex>();

            Map map = Map.MapLoader(args[0]);
            foreach (var node in map.Nodes.Values)
            {
                allVertices.Add(new Vertex(node.Id));
            }
            foreach (var vertex in allVertices)
            {
                foreach (var edge in map.Edges.Values.ToList().Where(x => x.Node1.Id == vertex.id || x.Node2.Id == vertex.id))
                {
                    if (edge.Node1.Id == vertex.id)
                    {
                        var otherVertex = allVertices.First(x => x.id == edge.Node2.Id);
                        vertex.adjacencies.Add(new TestOSM.Edge(otherVertex, edge.Distance));
                    }
                }
            }

            Node nA = FindNearestNearestNode(float.Parse(args[2]), float.Parse(args[3]), map.Nodes.Values.ToList());

            Node nZ = FindNearestNearestNode(float.Parse(args[4]), float.Parse(args[5]), map.Nodes.Values.ToList());

            Vertex A = allVertices.First(x => x.id == nA.Id);
            Vertex Z = allVertices.First(x => x.id == nZ.Id);
            //Vertex A = new Vertex(0);
            //Vertex B = new Vertex(1);
            //Vertex D = new Vertex(2);
            //Vertex F = new Vertex(3);
            //Vertex K = new Vertex(4);
            //Vertex J = new Vertex(5);
            //Vertex M = new Vertex(6);
            //Vertex O = new Vertex(7);
            //Vertex P = new Vertex(8);
            //Vertex R = new Vertex(9);
            //Vertex Z = new Vertex(10);

            //A.adjacencies.Add(new TestOSM.Edge(M, 8) );
            //B.adjacencies.Add(new TestOSM.Edge(D, 11));
            //D.adjacencies.Add(new TestOSM.Edge(B, 11));
            //F.adjacencies.Add(new TestOSM.Edge(K, 23));
            //K.adjacencies.Add(new TestOSM.Edge(O, 40));
            //J.adjacencies.Add(new TestOSM.Edge(K, 25));
            //M.adjacencies.Add(new TestOSM.Edge(R, 8) );
            //O.adjacencies.Add(new TestOSM.Edge(K, 40));
            //P.adjacencies.Add(new TestOSM.Edge(Z, 18));
            //R.adjacencies.Add(new TestOSM.Edge(P, 15));
            //Z.adjacencies.Add(new TestOSM.Edge(P, 18));


            TestOSM.Dijkstra.ComputePaths(A);
            Console.WriteLine("Distance to " + Z + ": " + Z.minDistance);
            List<Vertex> path = TestOSM.Dijkstra.GetShortestPathTo(Z);
            Console.Write("Path: ");
            foreach (var vertex in path)
            {
                Console.Write(vertex + ", ");
            }
        }

        private static Node FindNearestNearestNode(float lat, float lon, List<Node> nodes)
        {
            Func<Node, float, float, double> calculateDistance = (cnode, clat, clon) => {
                var sCoord = new GeoCoordinate(clat, clon);
                var eCoord = new GeoCoordinate(cnode.Latitude, cnode.Longitude);
                return Math.Abs(sCoord.GetDistanceTo(eCoord));
            };

            Node nearestNode = nodes.First();
            double nearestNodeDistance = calculateDistance(nearestNode, lat, lon);

            foreach (Node node in nodes)
            {
                double distance = calculateDistance(node, lat, lon);
                if (distance < nearestNodeDistance)
                {
                    nearestNode = node;
                    nearestNodeDistance = distance;
                }
            }
            return nearestNode;
        }
    }
}
