using System.Collections.Generic;

namespace EmCalculation
{
    public interface IEmAlgorithm
    {
        void Train(int maxIteration);
        int[] ClassifyDocuments();
        IEnumerable<int> GetWordsOrderedByMu(int cluster);
    }
}