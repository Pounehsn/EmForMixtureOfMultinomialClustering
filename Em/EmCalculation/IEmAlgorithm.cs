using System.Collections.Generic;

namespace EmCalculation
{
    public interface IEmAlgorithm
    {
        void Train(int maxIteration, int maxDurationInMinutess);
        int[] GetDocumentsCluster();
        IEnumerable<int> GetWordsOrderedByMu(int cluster);
    }
}