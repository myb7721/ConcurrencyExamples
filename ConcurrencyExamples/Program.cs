using Common;
using Parallelism;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Parallelism
{
    class Program
    {
        static double maxValue = 1E10;
        static int count = (int)1E2;

        static void Main(string[] args)
        {           
            //Data Paralellism
            var inputs = Utils.GetRandomPositiveNumbers(count, maxValue);

            //Will adjust thread usage to system conditions
            var a = new DataParallelism.CalculateSquareRoots.Calculator(new DataParallelism.CalculateSquareRoots.ParallelClassStrategy()).CalculateSquareRootsInParallel(inputs);
            WriteCalculateSquareRootsResultsToConsole(a);

            Console.WriteLine($"{Environment.NewLine}----------------------------------------------------------{Environment.NewLine}");

            //will try to use all available threads
            var b = new DataParallelism.CalculateSquareRoots.Calculator(new DataParallelism.CalculateSquareRoots.PLinqStrategy()).CalculateSquareRootsInParallel(inputs);
            WriteCalculateSquareRootsResultsToConsole(b);

            Console.WriteLine($"{Environment.NewLine}----------------------------------------------------------{Environment.NewLine}");

            //Task Paralellism
            Parallel.Invoke(
                 GetActionThatTakesRandomAmountOfTime(),
                 GetActionThatTakesRandomAmountOfTime(),
                 GetActionThatTakesRandomAmountOfTime(),
                 GetActionThatTakesRandomAmountOfTime()
                );

            Console.ReadLine();
        }

        private static void WriteCalculateSquareRootsResultsToConsole(IDictionary<int, List<Tuple<int, double>>> data)
        {
            foreach (var k in data.Keys)
            {
                Console.Write($"thread[{k}]: ");

                foreach (var t in data[k])
                {
                    Console.Write($"{t.Item1}, ");
                }
                Console.WriteLine();
            }
        }

        public static Action GetActionThatTakesRandomAmountOfTime()
        {
            return () =>
            {
                Console.WriteLine($"thread[{Thread.CurrentThread.ManagedThreadId}]: {DateTime.Now.TimeOfDay}");
                Thread.Sleep(new Random(Thread.CurrentThread.ManagedThreadId).Next((int)1E2, (int)1E4));
                Console.WriteLine($"thread[{Thread.CurrentThread.ManagedThreadId}]: {DateTime.Now.TimeOfDay}");
            };
        }
    }
}
