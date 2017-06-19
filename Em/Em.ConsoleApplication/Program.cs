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

            const int numberOfClusters = 5;
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

    public struct WordInfo
    {
        public WordInfo(int paperId, int documentId, int wordFrequency)
        {
            DocumentId = paperId;
            WordId = documentId;
            WordFrequency = wordFrequency;
        }

        public int DocumentId { get; }
        public int WordId { get; }
        public int WordFrequency { get; }
    }

    public class PaperInfoLoader : IDisposable
    {
        public PaperInfoLoader(FileInfo fileInfo)
        {
            _reader = fileInfo.OpenText();
        }

        private readonly StreamReader _reader;

        public IEnumerable<WordInfo> LoadWordInfo()
        {
            while (!_reader.EndOfStream)
            {
                var line = _reader.ReadLine();

                if (line == null)
                    continue;

                var lineParts = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                var paperIndex = int.Parse(lineParts[0]);
                foreach (var wordInfo in lineParts[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var wordInfoPrats = wordInfo.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                    var wordIndex = int.Parse(wordInfoPrats[0]);
                    var wordFrequency = int.Parse(wordInfoPrats[1]);

                    yield return new WordInfo(paperIndex, wordIndex, wordFrequency);
                }
            }
        }

        public void Dispose()
        {
            using (_reader) { }
        }
    }
}
