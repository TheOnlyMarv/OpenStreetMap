using System;
using System.Collections.Generic;

namespace OpenStreetMap
{
    class Program
    {
        static Dictionary<long, Node> myNode = new Dictionary<long, Node>();
        static List<Edge> myWays = new List<Edge>();
        static void Main(string[] args)
        {
            Map map = Map.MapLoader(@"map");
            Console.ReadLine();
        }
    }
}
