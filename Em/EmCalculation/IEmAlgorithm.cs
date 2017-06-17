namespace EmCalculation
{
    public interface IEmAlgorithm
    {
        void Train();
        int ClacifyDocument(int[] wordFrequency);
    }
}