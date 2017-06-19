using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

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
            Mu = new BigInteger[numberOfClusters, numberOfWords];
            Pi = new BigInteger[numberOfClusters];
            E = new BigInteger[numberOfDocuments, numberOfClusters];
            _nd = new BigInteger[numberOfDocuments];
            for (var dIndex = 0; dIndex < numberOfDocuments; dIndex++)
            {
                for (var wIndex = 0; wIndex < numberOfWords; wIndex++)
                {
                    _nd[dIndex] += wordInDocumentFrequency[dIndex, wIndex];
                }
            }
            _nk = new BigInteger[K];
        }

        private readonly Random _random;
        private readonly BigInteger[] _nd;
        private readonly BigInteger[] _nk;

        private const int Max = int.MaxValue;

        private int W { get; }
        private int D { get; }
        private int K { get; }
        private int[,] T { get; }
        private BigInteger[] Pi { get; }
        private BigInteger[,] Mu { get; }
        private BigInteger[,] E { get; }

        public void Train(int maxIteration)
        {
            Initialize();

            var previousePi = new BigInteger[K];
            var previouseMu = new BigInteger[K, W];
            var iteration = 0;
            for (; iteration < maxIteration; iteration++)
            {
                Array.Copy(Pi, previousePi, K);
                Array.Copy(Mu, previouseMu, K * W);

                IterationE(iteration);
                IterationM(iteration);

                if (IsConverged(previouseMu, previousePi)) break;
            }

            Log($"{nameof(Train)} completed in {iteration}");
        }

        public IEnumerable<int> GetWordsOrderedByMu(int cluster) => Enumerable.Range(0, D)
            .Select(i => new {index = i, mu = Mu[cluster, i]})
            .OrderByDescending(i=>i.mu)
            .Select(i=>i.index);

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

        private bool IsConverged(BigInteger[,] previouseMu, BigInteger[] previousePi)
        {
            BigInteger sum = 0;

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

            var changeFromPreviouseIteration = Math.Sqrt((double)sum);
            Log($"Change from previous iteration {changeFromPreviouseIteration}.");
            return changeFromPreviouseIteration < 1;
        }

        private void IterationE(int iteration)
        {
            Parallel.For(
                0,
                D,
                d =>
                {
                    Log($"Started iteration {iteration} document {d}.");
                    BigInteger sum = 0;
                    for (var k = 0; k < K; k++)
                    {
                        BigInteger mul = 1;
                        for (var w = 0; w < W; w++)
                        {
                            mul *= BigInteger.Pow(Mu[k, w], T[d, w]);
                        }
                        sum += E[d, k] = Pi[k] * mul;
                    }

                    for (var k = 0; k < K; k++)
                    {
                        E[d, k] = E[d, k] * Max / sum;
                    }
                }
            );

            Log($"{iteration} {nameof(IterationE)} completed");
        }

        private void IterationM(int iteration)
        {
            for (var k = 0; k < K; k++)
            {
                BigInteger sum = 0;
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
                    BigInteger sum = 1;
                    for (var d = 0; d < D; d++)
                    {
                        sum += E[d, k] * T[d, w];
                    }
                    Mu[k, w] = sum * Max / _nk[k];
                }
            }

            for (var k = 0; k < K; k++)
            {
                BigInteger sum = 1;
                for (var d = 0; d < D; d++)
                {
                    sum += E[d, k];
                }
                Pi[k] = sum / D;
            }

            Log($"{iteration} {nameof(IterationM)} completed");
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
            var initialValue = Max / K;
            for (var i = 0; i < K; i++)
            {
                Pi[i] = initialValue;
            }
        }

        private void InitializeMu()
        {
            for (var k = 0; k < K; k++)
            {
                BigInteger weightSum = 0;
                for (var w = 0; w < W; w++)
                {
                    var weight = Mu[k, w] = NextRandomFrom25To75Percent();
                    weightSum += weight;
                }
                for (var w = 0; w < W; w++)
                {
                    Mu[k, w] = Mu[k, w] * Max / weightSum;
                }
            }
        }

        private int NextRandomFrom25To75Percent() =>
            _random.Next(0, Max) / 2 + Max / 4;
    }
}
