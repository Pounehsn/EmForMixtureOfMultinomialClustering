using System;
using System.Collections.Generic;
using System.IO;

namespace Em.ConsoleApplication
{
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