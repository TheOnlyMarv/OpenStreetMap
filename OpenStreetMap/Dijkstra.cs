using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStreetMap
{
    public class Dijkstra
    {
        public float StartLat { get; set; }
        public float StartLon { get; set; }
        public float EndLat { get; set; }
        public float EndLon { get; set; }
        public Map Map { get; set; }

        public Node StartNode { get; set; }
        public Node EndNode { get; set; }

        public Dijkstra(Map map, float startLat, float startLon, float endLat, float endLon)
        {
            this.Map = map;
            this.StartLat = startLat;
            this.StartLon = startLon;
            this.EndLat = endLat;
            this.EndLon = endLon;

            FindNearestStartEndNodes();
            long[] pi = null;
            List<Node> nodeList = Map.Nodes.Values.ToList();
            Diijkstra(ref pi, ref nodeList, StartNode.Id);
            CalculateShortestPath();
        }

        private void FindNearestStartEndNodes()
        {
            this.StartNode = FindNearestNearestNode(StartLat, StartLon);
            this.EndNode = FindNearestNearestNode(EndLat, EndLon);
        }

        private Node FindNearestNearestNode(float lat, float lon)
        {
            Func<Node, float, float, double> calculateDistance = (cnode, clat, clon) => {
                var sCoord = new GeoCoordinate(clat, clon);
                var eCoord = new GeoCoordinate(cnode.Latitude, cnode.Longitude);
               return Math.Abs(sCoord.GetDistanceTo(eCoord));
            };

            Node nearestNode = Map.Nodes.Select(x => x.Value).First();
            double nearestNodeDistance = calculateDistance(nearestNode, lat, lon);

            foreach (Node node in Map.Nodes.Select(x=>x.Value))
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

        
        public List<long> Diijkstra(ref long[] pi, ref List<Node> G, long s)
        {
            InitializeSingleSource(ref pi, ref G, s);

            List<long> S = new List<long>();
            PriorityQueue Q = new PriorityQueue(G);

            Q.buildHeap();

            while (Q.size() != 0)
            {
                Node u = Q.extractMin();

                S.Add(u.Id);

                for (int i = 0; i < u.OtherNeighbors.Count; i++)
                {

                    Node v = u.OtherNeighbors[i];
                    double w = u.Weights[i];

                    Relax(ref pi, u, ref v, w);
                }
            }

            return S;
        }

        void InitializeSingleSource(ref long[] pi, ref List<Node> nodeList, long s)
        {
            pi = new long[nodeList.Count];

            for (int i = 0; i < pi.Length; i++)
                pi[i] = -1;

            Node node = nodeList.First(x => x.Id == s);
            node.Distance = 0;
            nodeList.Insert(0, node);
        }

        void Relax(ref long[] pi, Node u, ref Node v, double w)
        {
            if (v.Distance > u.Distance + w)
            {
                v.Distance = u.Distance + w;
                pi[v.Id] = u.Id;
            }
        }






        /// <summary>
        /// //////////
        /// </summary>
        List<Node> nodes;
        List<Edge> edges;


        HashSet<Node> settkedNodes;
        HashSet<Node> unSettledNodes;
        Dictionary<Node, double> distance;
        Dictionary<Node, Node> predecessors;

        private void CalculateShortestPath()
        {
            nodes = Map.Nodes.Select(x => x.Value).ToList();
            edges = Map.Edges.Select(x => x.Value).ToList();


            settkedNodes = new HashSet<Node>();
            unSettledNodes = new HashSet<Node>();
            distance = new Dictionary<Node, double>();
            predecessors = new Dictionary<Node, Node>();

            distance.Add(StartNode, 0);
            unSettledNodes.Add(StartNode);
            while (unSettledNodes.Count > 0)
            {
                Node node = GetMinimum(unSettledNodes);
                settkedNodes.Add(node);
                unSettledNodes.Remove(node);
                FindMinimalDistances(node);
            }
        }

        private void FindMinimalDistances(Node node)
        {
            List<Node> adjecentNodes = node.Neighbors.Select(x => x.Value).ToList();
            foreach (Node target in adjecentNodes)
            {
                if(GetShortestDistance(target)>GetShortestDistance(node) + GetDistance(node, target))
                {
                    predecessors.Add(target, node);
                    unSettledNodes.Add(target);
                }
            }
        }

        private double GetDistance(Node source, Node target)
        {
            foreach (Edge edge in edges)
            {
                if ((edge.Node1 == source && edge.Node2 == target) ||
                    (edge.Node1 == target && edge.Node2 == source))
                {
                    return edge.Distance;
                }
            }
            throw new Exception("Should not happen");
        }

        private Node GetMinimum(HashSet<Node> nodes)
        {
            Node minimum = null;
            foreach (Node node in nodes)
            {
                if (minimum == null)
                {
                    minimum = node;
                }
                else
                {
                    if (GetShortestDistance(node)<GetShortestDistance(minimum))
                    {
                        minimum = node;
                    }
                }
            }
            return minimum;
        }

        private double GetShortestDistance(Node node)
        {
            double d = double.MaxValue;
            try
            {
                d = distance[node];

            }
            catch (Exception)
            {
            }

            return d;
        }


        public List<Node> GetPath()
        {
            List<Node> path = new List<Node>();
            Node step = this.EndNode;
            try
            {
                Node tempstep = predecessors[step];
                path.Add(step);

            }
            catch (Exception)
            {
                return null;
            }

            if (predecessors[step] == null)
            {
                return null;
            }
            path.Add(step);
            while (predecessors[step] != null)
            {
                step = predecessors[step];
                path.Add(step);
            }
            path.Reverse();
            return path;
        }
    }
}
