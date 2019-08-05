using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Asynchrony
{
    class Program
    {
        static void Main(string[] args)
        {
            var executionTime = Utils.GetRandomPositiveNumbers(3, 1E5);
           
            //Both async and potentially parallel;
           Task.WhenAll(Test(), Test(), Test()).Wait();

            Console.ReadKey();
        }

        private static async Task Test()
        {
            await Task.Run(async () =>
           {
               Console.WriteLine($"thread[{Thread.CurrentThread.ManagedThreadId}]: {DateTime.Now.TimeOfDay}");
               await Task.Delay(new Random(Thread.CurrentThread.ManagedThreadId).Next((int)1E2, (int)1E4));
               Console.WriteLine($"thread[{Thread.CurrentThread.ManagedThreadId}]: {DateTime.Now.TimeOfDay}");
           });
        }
    }
}
