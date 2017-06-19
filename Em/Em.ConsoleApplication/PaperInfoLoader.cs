using System;
using System.Collections.Generic;
using System.IO;

namespace Em.ConsoleApplication
{
    public class PaperInfoLoader : IDisposable
    {
        public PaperInfoLoader(FileInfo papersFile, FileInfo wordsFile)
        {
            _wordsReader = wordsFile.OpenText();
            _papersReader = papersFile.OpenText();
        }


        private readonly StreamReader _papersReader;
        private readonly StreamReader _wordsReader;

        public IEnumerable<Word> LoadWords()
        {
            while (!_wordsReader.EndOfStream)
            {
                var line = _papersReader.ReadLine();

                if (line == null)
                    continue;

                var lineParts = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                yield return new Word(
                    id: int.Parse(lineParts[0]),
                    text: lineParts[1],
                    frequency: int.Parse(lineParts[2])
                );
            }
        }

        public IEnumerable<WordUsageInfo> LoadDocumentInfo()
        {
            while (!_papersReader.EndOfStream)
            {
                var line = _papersReader.ReadLine();

                if (line == null)
                    continue;

                var lineParts = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                var paperIndex = int.Parse(lineParts[0]);
                foreach (var wordInfo in lineParts[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var wordInfoPrats = wordInfo.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                    var wordIndex = int.Parse(wordInfoPrats[0]);
                    var wordFrequency = int.Parse(wordInfoPrats[1]);

                    yield return new WordUsageInfo(paperIndex, wordIndex, wordFrequency);
                }
            }
        }

        public void Dispose()
        {
            using (_papersReader) { }
            using (_wordsReader) { }
        }
    }
}