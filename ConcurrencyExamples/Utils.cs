using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Parallelism
{
    public static class Utils
    {
        public static List<double> GetRandomPositiveNumbers(int count, double max)
        {
            var numbers = new List<double>();
            for (int i = 0; i < count; i++)
            {
                numbers.Add(new Random(i).NextDouble() * max);
            }

            return numbers;
        }

        
    }
}
