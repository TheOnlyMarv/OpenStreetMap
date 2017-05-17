using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Priority_Queue;

namespace TestOSM
{
    class Vertex : IComparable<Vertex>
    {
        public long id;
        public double minDistance = Double.PositiveInfinity;
        public List<Edge> adjacencies;
        public Vertex previous;
        public Vertex(long id)
        {
            this.id = id;
            adjacencies = new List<Edge>();
        }
        override public string ToString()
        {
            return id.ToString();
        }

        public int CompareTo(Vertex other)
        {
            return minDistance.CompareTo(other.minDistance);
        }
    }

    class Edge
    {
        public Vertex target;
        public double weight;
        public Edge(Vertex target, double weight)
        {
            this.target = target;
            this.weight = weight;
        }
    }

    class Dijkstra
    {
        public static void ComputePaths(Vertex source)
        {
            source.minDistance = 0;
            SortedSet<Vertex> vertexQueue = new SortedSet<Vertex>();
            vertexQueue.Add(source);

            while (vertexQueue.Count != 0)
            {
                Vertex u = vertexQueue.First();
                vertexQueue.Remove(u);

                foreach (Edge e in u.adjacencies)
                {
                    Vertex v = e.target;
                    double weigth = e.weight;
                    double distanceThroughU = u.minDistance + weigth;
                    if (distanceThroughU < v.minDistance)
                    {
                        vertexQueue.Remove(v);

                        v.minDistance = distanceThroughU;
                        v.previous = u;
                        vertexQueue.Add(v);
                    }
                }
            }
        }

        public static List<Vertex> GetShortestPathTo(Vertex target)
        {
            List<Vertex> path = new List<Vertex>();
            for (Vertex vertex = target; vertex != null; vertex = vertex.previous)
            {
                path.Add(vertex);
            }
            path.Reverse();
            return path;
        }
    }
}
