using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EmCalculation;

namespace Em.ConsoleApplication
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 1 && args[0].Contains("default"))
                args = new[]
                {
                    "5",
                    "../../../nips.vocab",
                    "../../../nips.libsvm",
                    "../../../output-100.txt"
                };

            if (args.Length != 4)
            {
                Console.WriteLine("Em.ConsoleApplication <number-of-clusters> <word-input-file> <paper-input-file> <output-file>");
                return;
            }

            var numberOfClusters = int.Parse(args[0]);
            var wordInputFile = args[1];
            var paperInputFile = args[2];
            var outputFile = args[3];
            var wordIdToString = new Dictionary<int, string>();

            var maxDocumentId = 0;
            var maxWordId = 0;
            using (var loder = new PaperInfoLoader(new FileInfo(paperInputFile), new FileInfo(wordInputFile)))
            {
                foreach (var wordInfo in loder.LoadDocumentInfo())
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

            using (var loder = new PaperInfoLoader(new FileInfo(paperInputFile), new FileInfo(wordInputFile)))
            {
                foreach (var wordInfo in loder.LoadDocumentInfo())
                {
                    wordInDocumentFrequency[wordInfo.DocumentId, wordInfo.WordId] = wordInfo.WordFrequency;
                }

                Console.WriteLine("Documents are loaded.");

                var count = 0;

                foreach (var word in loder.LoadWords())
                {
                    if (count++ % 100 == 0)
                        Console.WriteLine($"{count} Words are loaded.");
                    wordIdToString[word.Id] = word.Text;
                }

                Console.WriteLine("All words are loaded.");
            }

            var em = new EmAlgorithm(numberOfClusters, wordInDocumentFrequency, 0);

            em.Train(1000);

            using (var textWriter = new FileInfo(outputFile).CreateText())
            {
                for (var k = 0; k < numberOfClusters; k++)
                {
                    textWriter.WriteLine(
                        $"{k} : {string.Join(", ", em.GetWordsOrderedByMu(k).Take(10).Select(i => wordIdToString[i]))}"
                    );
                }
            }

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
