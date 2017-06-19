using System;
using System.IO;
using System.Linq;
using EmCalculation;

namespace Em.ConsoleApplication
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            const string inputFile = @"..\..\..\nips.libsvm";
            const string outputFile = @"..\..\..\result.txt";

            var maxDocumentId = 0;
            var maxWordId = 0;
            using (var loder = new PaperInfoLoader(new FileInfo(inputFile)))
            {
                foreach (var wordInfo in loder.LoadWordInfo())
                {
                    if (wordInfo.WordId > maxWordId)
                        maxWordId = wordInfo.WordId;
                    if (wordInfo.DocumentId > maxDocumentId)
                        maxDocumentId = wordInfo.DocumentId;
                }
            }

            var numberOfClusters = int.Parse(args[0]);
            var wordInDocumentFrequency = new int[maxDocumentId + 1, maxWordId + 1];

            for (var i = 0; i < maxDocumentId + 1; i++)
            {
                for (var j = 0; j < maxWordId + 1; j++)
                {
                    wordInDocumentFrequency[i, j] = 1;
                }
            }

            using (var loder = new PaperInfoLoader(new FileInfo(inputFile)))
            {
                foreach (var wordInfo in loder.LoadWordInfo())
                {
                    wordInDocumentFrequency[wordInfo.DocumentId, wordInfo.WordId] = wordInfo.WordFrequency;
                }
            }

            var em = new EmAlgorithm(numberOfClusters, wordInDocumentFrequency, 0);

            em.Train(1000);

            var classified = em.ClassifyDocuments();

            using (var textWriter = new FileInfo(outputFile).CreateText())
            {
                textWriter.WriteLine(
                    string.Join(" ", classified.Select((v, i) => $"{i}:{v}"))
                );
            }

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
