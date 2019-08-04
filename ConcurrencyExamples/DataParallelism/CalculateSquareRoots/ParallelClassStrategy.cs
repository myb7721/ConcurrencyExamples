using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Parallelism.DataParallelism.CalculateSquareRoots
{
    //Will adjust number of processors to system conditions
    public class ParallelClassStrategy : IStrategy
    {
        object countLock = new object();

        public IDictionary<int, List<Tuple<int, double>>> CalculateSquareRootsInParallel(IEnumerable<double> inputs)
        {   
            var data = new ConcurrentDictionary<int, List<Tuple<int, double>>>();
        
            int count = 0;

            Parallel.ForEach(inputs, i =>
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
