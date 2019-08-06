using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parallelism.DataParallelism.CalculateSquareRoots
{
    public class Calculator
    {
        private IStrategy strategy;

        public Calculator(IStrategy strategy)
        {
            this.strategy = strategy;
        }

        public IDictionary<int, List<Tuple<int, double>>> CalculateSquareRootsInParallel(IEnumerable<double> inputs)
        {
            return strategy.CalculateSquareRootsInParallel(inputs);
        }
    }
}
