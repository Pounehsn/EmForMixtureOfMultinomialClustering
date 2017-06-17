using System;

namespace EmCalculation
{
    public class EmAlgorithm : IEmAlgorithm
    {
        public EmAlgorithm(int numberOfClusters, int[,] wordInDocumentFrequency, int randomSeed)
        {
            _random = new Random(randomSeed);
            K = numberOfClusters;
            T = wordInDocumentFrequency;
            var numberOfDocuments = D = wordInDocumentFrequency.GetLength(0);
            var numberOfWords = W = wordInDocumentFrequency.GetLength(1);
            Mu = new double[numberOfClusters, numberOfWords];
            Pi = new double[numberOfClusters];
            E = new double[numberOfClusters, numberOfDocuments];
        }

        private readonly Random _random;

        private int W { get; }
        private int D { get; }
        private int K { get; }
        private int[,] T { get; }
        private double[] Pi { get; }
        private double[,] Mu { get; }
        private double[,] E { get; }

        public void Train(int maxIteration)
        {
            Initialize();

            var previousePi = new double[K];
            var previouseMu = new double[K, W];

            for (var iteration = 0; iteration < maxIteration; iteration++)
            {
                Array.Copy(Pi, previousePi, K);
                Array.Copy(Mu, previouseMu, K * W);

                IterationE();
                IterationM();

                if (
                    IsConverged(
                        previouseMu,
                        previousePi
                    )
                )
                    break;
            }
        }

        private bool IsConverged(double[,] previouseMu, double[] previousePi)
        {
            var sum = 0.0;

            for (var k = 0; k < K; k++)
            {
                var dPi = previousePi[k] - Pi[k];
                sum += dPi * dPi;
                for (var w = 0; w < W; w++)
                {
                    var dMu = previouseMu[k, w] - Mu[k, w];
                    sum += dMu * dMu;
                }
            }

            return Math.Sqrt(sum) < 1E-6;
        }

        private void IterationM()
        {
            throw new NotImplementedException();
        }

        private void IterationE()
        {
            throw new NotImplementedException();
        }

        public int[] ClassifyDocument()
        {
            throw new NotImplementedException();
        }

        private void Initialize()
        {
            InitializePi();
            InitializeMu();
        }

        private void InitializePi()
        {
            for (var i = 0; i < K - 1; i++)
            {
                Pi[i] = 1.0 / K;
            }
        }

        private void InitializeMu()
        {
            for (var k = 0; k < K; k++)
            {
                var weightSum = 0.0;
                for (var w = 0; w < W; w++)
                {
                    var weight = Mu[k, w] = NextRandomFrom25To75Percent();
                    weightSum += weight;
                }
                for (var w = 0; w < W; w++)
                {
                    Mu[k, w] /= weightSum;
                }
            }
        }

        private double NextRandomFrom25To75Percent() =>
            _random.NextDouble() / 2 + 0.25;
    }
}
