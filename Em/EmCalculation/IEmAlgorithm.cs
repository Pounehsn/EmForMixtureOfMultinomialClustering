using System.Collections.Generic;
using System.Threading;

namespace EmCalculation
{
    public interface IEmAlgorithm
    {
        void Train(int maxIteration, int maxDurationInMinutess, CancellationToken token);
        int[] GetDocumentsCluster();
        IEnumerable<int> GetWordsOrderedByMu(int cluster);
    }
}