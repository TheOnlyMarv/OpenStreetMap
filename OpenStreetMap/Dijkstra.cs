using System;
using System.Collections.Generic;
using System.Linq;

using System.Device.Location;

namespace OpenStreetMap
{
    public class Dijkstra
    {
        public static Vertex FindNearestNearestNode(float lat, float lon, List<Vertex> allVertices)
        {
            Func<Vertex, float, float, double> calculateDistance = (cnode, clat, clon) =>
            {
                var sCoord = new GeoCoordinate(clat, clon);
                var eCoord = new GeoCoordinate(cnode.Latitude, cnode.Longitude);
                return Math.Abs(sCoord.GetDistanceTo(eCoord));
            };

            Vertex nearestNode = allVertices.First();
            double nearestNodeDistance = calculateDistance(nearestNode, lat, lon);

            foreach (Vertex node in allVertices)
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

        public static void ComputePaths(Vertex source)
        {
            source.MinDistance = 0;
            SortedSet<Vertex> vertexQueue = new SortedSet<Vertex>();
            vertexQueue.Add(source);

            while (vertexQueue.Count != 0)
            {
                Vertex u = vertexQueue.First();
                vertexQueue.Remove(u);

                foreach (Edge e in u.Adjacencies)
                {
                    Vertex v = e.Target;
                    double weigth = e.Weight;
                    double distanceThroughU = u.MinDistance + weigth;
                    if (distanceThroughU < v.MinDistance)
                    {
                        vertexQueue.Remove(v);

                        v.MinDistance = distanceThroughU;
                        v.Previous = u;
                        vertexQueue.Add(v);
                    }
                }
            }
        }

        public static List<Vertex> GetShortestPathTo(Vertex target)
        {
            List<Vertex> path = new List<Vertex>();
            for (Vertex vertex = target; vertex != null; vertex = vertex.Previous)
            {
                path.Add(vertex);
            }
            path.Reverse();
            return path;
        }
    }
}
