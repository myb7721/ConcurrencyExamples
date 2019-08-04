using Parallelism;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parallelism
{
    class Program
    {
        static double maxValue = 1E10;
        static int count = (int)1E2;
        static bool excludeValueFromConsole = true;


        static void Main(string[] args)
        {
            var inputs = Utils.GetRandomPositiveNumbers(count, maxValue);

            var a = new DataParallelism.CalculateSquareRoots.Calculator(new DataParallelism.CalculateSquareRoots.ParallelClassStrategy()).CalculateSquareRootsInParallel(inputs);
            WriteCalculateSquareRootsResultsToConsole(a);

            Console.WriteLine($"{Environment.NewLine}----------------------------------------------------------{Environment.NewLine}");
            var b = new DataParallelism.CalculateSquareRoots.Calculator(new DataParallelism.CalculateSquareRoots.PLinqStrategy()).CalculateSquareRootsInParallel(inputs);
            WriteCalculateSquareRootsResultsToConsole(b);

            Console.ReadLine();
        }


        private static void WriteCalculateSquareRootsResultsToConsole(IDictionary<int, List<Tuple<int, double>>> data)
        {

            foreach (var k in data.Keys)
            {
                Console.Write($"thread[{k}]: ");

                foreach (var t in data[k])
                {
                    var calculatedValue = excludeValueFromConsole ? string.Empty : string.Join(",", t.Item2);
                    Console.Write($"order[{t.Item1}] {calculatedValue}, ");
                }
                Console.WriteLine();
            }
        }

    }
}
