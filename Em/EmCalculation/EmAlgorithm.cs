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
            E = new double[numberOfDocuments, numberOfClusters];
            _nd = new int[numberOfDocuments];
            for (var dIndex = 0; dIndex < numberOfDocuments; dIndex++)
            {
                for (var wIndex = 0; wIndex < numberOfWords; wIndex++)
                {
                    _nd[dIndex] += wordInDocumentFrequency[dIndex, wIndex];
                }
            }
            _nk = new double[K];
        }

        private readonly Random _random;
        private readonly int[] _nd;
        private readonly double[] _nk;

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
            var iteration = 0;
            for (; iteration < maxIteration; iteration++)
            {
                Array.Copy(Pi, previousePi, K);
                Array.Copy(Mu, previouseMu, K * W);

                IterationE(iteration);
                IterationM(iteration);

                if (
                    IsConverged(
                        previouseMu,
                        previousePi
                    )
                )
                    break;
            }

            Log($"{nameof(Train)} completed in {iteration}");
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

            var changeFromPreviouseIteration = Math.Sqrt(sum);
            Log($"Change from previous iteration {changeFromPreviouseIteration}.");
            return changeFromPreviouseIteration < 1E-6;
        }

        private void IterationE(int iteration)
        {
            for (var d = 0; d < D; d++)
            {
                var sum = 0.0;
                for (var k = 0; k < K; k++)
                {
                    var mul = 1.0;
                    for (var w = 0; w < W; w++)
                    {
                        mul *= Math.Pow(Mu[k, w], T[d, w]);
                    }
                    sum += E[d, k] = Pi[k] * mul;
                }

                for (var k = 0; k < K; k++)
                {
                    E[d, k] /= sum;
                }
            }

            Log($"{iteration} {nameof(IterationE)} completed");
        }

        private void IterationM(int iteration)
        {
            for (var k = 0; k < K; k++)
            {
                var sum = 0.0;
                for (var d = 0; d < D; d++)
                {
                    sum += E[d, k] * _nd[d];
                }
                _nk[k] = sum;
            }

            for (var k = 0; k < K; k++)
            {
                for (var w = 0; w < W; w++)
                {
                    var sum = 0.0;
                    for (var d = 0; d < D; d++)
                    {
                        sum += E[d, k] * T[d, w];
                    }
                    Mu[k, w] = sum / _nk[k];
                }
            }

            for (var k = 0; k < K; k++)
            {
                var sum = 0.0;
                for (var d = 0; d < D; d++)
                {
                    sum += E[d, k];
                }
                Pi[k] = sum / D;
            }

            Log($"{iteration} {nameof(IterationM)} completed");
        }

        public int[] ClassifyDocuments()
        {
            var result = new int[D];

            for (var d = 0; d < D; d++)
            {
                var clusterIndex = 0;
                var clusterExpectation = E[d, clusterIndex];
                for (var k = 1; k < K; k++)
                {
                    if (!(E[d, k] > clusterExpectation))
                        continue;

                    clusterExpectation = E[d, k];
                    clusterIndex = k;
                }

                result[d] = clusterIndex;
            }

            Log($"{nameof(ClassifyDocuments)} is completed");
            return result;
        }

        private void Initialize()
        {
            InitializePi();
            Log($"{nameof(InitializePi)} is completed");
            InitializeMu();
            Log($"{nameof(InitializeMu)} is completed");
        }

        private void Log(string message)
        {
            Console.WriteLine(message);
        }

        private void InitializePi()
        {
            var initialValue = 1.0 / K;
            for (var i = 0; i < K ; i++)
            {
                Pi[i] = initialValue;
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
