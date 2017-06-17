namespace EmCalculation
{
    public interface IEmAlgorithm
    {
        void Train(int maxIteration);
        int[] ClassifyDocument();
    }
}