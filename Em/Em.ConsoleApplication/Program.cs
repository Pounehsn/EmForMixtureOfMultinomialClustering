using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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


            var maxNumberOfIteration = int.Parse(ConfigurationManager.AppSettings.Get("MaxNumberOfIteration"));
            var maxDurationInMinutess = int.Parse(ConfigurationManager.AppSettings.Get("MaxDurationInMinutess"));
            var maxScale = int.Parse(ConfigurationManager.AppSettings.Get("MaxScale"));

            Console.WriteLine($"{nameof(maxNumberOfIteration)} : {maxNumberOfIteration}");
            Console.WriteLine($"{nameof(maxDurationInMinutess)} : {maxDurationInMinutess}");
            Console.WriteLine($"{nameof(maxScale)} : {maxScale}");

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

                foreach (var word in loder.LoadWords())
                {
                    wordIdToString[word.Id] = word.Text;
                }

                Console.WriteLine("All words are loaded.");
            }

            var em = new EmAlgorithm(numberOfClusters, wordInDocumentFrequency, maxScale, 0);

            var trainingEnded = false;
            var cancellationTokenSource = new CancellationTokenSource();
            var task = Task.Run(
                () =>
                {
                    var quit = false;
                    while (!trainingEnded & !quit)
                    {
                        Console.WriteLine("Enter 'q' to finish training.");
                        quit = Console.ReadLine()?.ToUpper() == "Q";

                        if(quit)
                            cancellationTokenSource.Cancel();
                    }
                }
            );

            em.Train(maxNumberOfIteration, maxDurationInMinutess, cancellationTokenSource.Token);

            trainingEnded = true;

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
            task.Wait(cancellationTokenSource.Token);
            Console.ReadLine();
        }
    }
}
