using System;

namespace OpenStreetMap
{
    class Program
    {
        static void Main(string[] args)
        {
            Map map = Map.MapLoader(args[0]);
            map.ExportToGraphiv(args[1], 1000, "0.000000");
            Console.ReadLine();
        }
    }
}
