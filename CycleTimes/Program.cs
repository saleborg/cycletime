using System;
using ThroughputCalculation.GetTheData;

namespace ThroughputCalculation
{
    class Program
    {
        static void Main(string[] args)
        {
            new GetDataFromVsts();
            Console.Write("Press any key to continue...");
            Console.ReadKey(true);
        }
    }
}
