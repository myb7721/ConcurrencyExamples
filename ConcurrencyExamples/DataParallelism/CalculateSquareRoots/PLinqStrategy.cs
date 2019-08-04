using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Parallelism.DataParallelism.CalculateSquareRoots
{
    //Will assume it can use all processors
    internal class PLinqStrategy : IStrategy
    {
        object countLock = new object();

        public IDictionary<int, List<Tuple<int, double>>> CalculateSquareRootsInParallel(IEnumerable<double> inputs)
        {
            var data = new ConcurrentDictionary<int, List<Tuple<int, double>>>();
       
            int count = 0;
            inputs.AsParallel().ForAll(i =>
            {
                var sqrt = Math.Sqrt(i);
                var threadId = Thread.CurrentThread.ManagedThreadId;

                if (data.TryGetValue(threadId, out var list))
                {
                    list.Add(new Tuple<int, double>(count, sqrt));
                }
                else
                {
                    data[threadId] = new List<Tuple<int, double>> { new Tuple<int, double>(count, sqrt) };
                }

                lock (countLock)
                {
                    count++;
                }
            });

            return data;
        }
    }
}
