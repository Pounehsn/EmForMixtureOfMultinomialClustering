using System;

namespace EmCalculation
{
    public class EmAlgorithm : IEmAlgorithm
    {
        public EmAlgorithm(int numberOfClusters, int[,] wordInDocumentFrequency)
        {
            K = numberOfClusters;
            T = wordInDocumentFrequency;
            NumberOfDocuments = wordInDocumentFrequency.GetLength(0);
            var numberOfWords = NumberOfWords = wordInDocumentFrequency.GetLength(1);
            Mu = new double[numberOfClusters, numberOfWords];
            Pi = new double[numberOfClusters];
        }

        private int NumberOfWords { get; }
        private int NumberOfDocuments { get; }
        private int K { get; }
        private int[,] T { get; }
        private double[] Pi { get; }
        private double[,] Mu { get; }

        public void Train()
        {
            
        }

        public int ClacifyDocument(int[] wordFrequency)
        {
            throw new NotImplementedException();
        }
    }
}
