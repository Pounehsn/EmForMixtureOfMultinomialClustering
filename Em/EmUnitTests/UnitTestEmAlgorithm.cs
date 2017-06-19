using System.Linq;
using System.Threading;
using EmCalculation;
using NUnit.Framework;

namespace EmUnitTests
{
    [TestFixture]
    public class UnitTestEmAlgorithm
    {
        [Test]
        public void DumeTest()
        {
            Assert.AreEqual(4, 2 * 2);
        }

        [Test]
        public void ClassifyDocument_FixInputs_ShouldClassifyAsExpected()
        {
            //Arrange
            var algorithm = new EmAlgorithm(
                2,
                new[,]
                {
                    { 5, 5 }, // expected to go in cluster 1
                    { 9, 1 }, // expected to go in cluster 2
                    { 8, 2 }, // expected to go in cluster 2
                    { 4, 6 }, // expected to go in cluster 1
                    { 7, 3 }  // expected to go in cluster 2
                }, 
                int.MaxValue,
                1
            );

            algorithm.Train(100, 100, CancellationToken.None);

            //Act
            var documentClass = algorithm.GetDocumentsCluster();

            //Assert
            Assert.AreEqual(1, documentClass[0]);
            Assert.AreEqual(0, documentClass[1]);
            Assert.AreEqual(0, documentClass[2]);
            Assert.AreEqual(1, documentClass[3]);
            Assert.AreEqual(0, documentClass[4]);
        }

        [Test]
        public void GetWordsOrderedByMu_FixInputs_ShouldOrderAsExpected()
        {
            //Arrange
            var algorithm = new EmAlgorithm(
                2,
                new[,]
                {
                    { 5, 5 }, // expected to go in cluster 1
                    { 9, 1 }, // expected to go in cluster 2
                    { 8, 2 }, // expected to go in cluster 2
                    { 4, 6 }, // expected to go in cluster 1
                    { 7, 3 }  // expected to go in cluster 2
                },
                int.MaxValue,
                1
            );

            algorithm.Train(100, 100, CancellationToken.None);

            //Act
            var wordsForK1 = algorithm.GetWordsOrderedByMu(0).ToArray();
            var wordsForK2 = algorithm.GetWordsOrderedByMu(1).ToArray();

            //Assert
            Assert.AreEqual(2, wordsForK1.Length);
            Assert.AreEqual(0, wordsForK1[0]);
            Assert.AreEqual(1, wordsForK1[1]);
            Assert.AreEqual(2, wordsForK2.Length);
            Assert.AreEqual(0, wordsForK2[0]);
            Assert.AreEqual(1, wordsForK2[1]);
        }
    }
}
