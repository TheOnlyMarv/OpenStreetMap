using System;

namespace OpenStreetMap
{
    class Program
    {
        static void Main(string[] args)
        {
            Map map = Map.MapLoader(args[0]);
            Console.ReadLine();
        }
    }
}
