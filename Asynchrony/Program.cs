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
            Task t1 = Task.Run(
                () =>
                {
                    Console.WriteLine($"thread[{Thread.CurrentThread.ManagedThreadId}]: {DateTime.Now.TimeOfDay}");
                    Thread.Sleep(new Random(Thread.CurrentThread.ManagedThreadId).Next((int)1E2, (int)1E4));
                    Console.WriteLine($"thread[{Thread.CurrentThread.ManagedThreadId}]: {DateTime.Now.TimeOfDay}");
                });

            Task t2 = Task.Run(
               () =>
               {
                   Console.WriteLine($"thread[{Thread.CurrentThread.ManagedThreadId}]: {DateTime.Now.TimeOfDay}");
                   Thread.Sleep(new Random(Thread.CurrentThread.ManagedThreadId).Next((int)1E2, (int)1E4));
                   Console.WriteLine($"thread[{Thread.CurrentThread.ManagedThreadId}]: {DateTime.Now.TimeOfDay}");
               });

            Task t3 = Task.Run(
               () =>
               {
                   Console.WriteLine($"thread[{Thread.CurrentThread.ManagedThreadId}]: {DateTime.Now.TimeOfDay}");
                   Thread.Sleep(new Random(Thread.CurrentThread.ManagedThreadId).Next((int)1E2, (int)1E4));
                   Console.WriteLine($"thread[{Thread.CurrentThread.ManagedThreadId}]: {DateTime.Now.TimeOfDay}");
               });

            Task.Run(()=>Task.WhenAll(t1, t2, t2));

            //this will dead lock ui
            Task.WhenAll(t1, t2, t2).Wait();

            Console.ReadKey();
        }
    }
}
