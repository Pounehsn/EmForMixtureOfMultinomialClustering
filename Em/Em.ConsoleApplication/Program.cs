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
            if (args.Length != 3)
            {
                Console.WriteLine("Em.ConsoleApplication <number-of-clusters> <input-file> <output-file>");
                return;
            }

            var numberOfClusters = int.Parse(args[0]);
            var inputFile = args[1];
            var outputFile = args[2];

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

            using (var textWriter = new FileInfo(outputFile).CreateText())
            {
                for (var k = 0; k < numberOfClusters; k++)
                {
                    textWriter.WriteLine(
                        $"{k} : {string.Join("\t", em.GetWordsOrderedByMu(k).Take(10))}"
                    );
                }
            }

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
