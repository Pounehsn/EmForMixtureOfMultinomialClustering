namespace Em.ConsoleApplication
{
    public struct WordUsageInfo
    {
        public WordUsageInfo(int paperId, int documentId, int wordFrequency)
        {
            DocumentId = paperId;
            WordId = documentId;
            WordFrequency = wordFrequency;
        }

        public int DocumentId { get; }
        public int WordId { get; }
        public int WordFrequency { get; }
    }
}