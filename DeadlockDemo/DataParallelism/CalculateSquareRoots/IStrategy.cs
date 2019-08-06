using System;
using System.Collections.Generic;

namespace Parallelism.DataParallelism.CalculateSquareRoots
{
    public interface IStrategy
    {
        IDictionary<int, List<Tuple<int, double>>> CalculateSquareRootsInParallel(IEnumerable<double> inputs);   
    }
}
