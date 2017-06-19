namespace Em.ConsoleApplication
{
    public struct Word
    {
        public Word(int id, string text, int frequency)
        {
            this.Id = id;
            this.Text = text;
            this.Frequency = frequency;
        }

        public int Frequency { get; }
        public int Id { get; }
        public string Text { get; }
    }
}