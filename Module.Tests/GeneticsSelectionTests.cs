using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FortsRobotLib.CandleProviders;
using BasicAlgorithms;
using System.Linq;
using System.IO;
using FortsRobotLib.AccAggregator;
using FortsRobotLib.Algorithms;
using FortsRobotLib.Genetics;

namespace Module.Tests
{
    [TestClass]
    public class GeneticsSelectionTests
    {
        [TestMethod]
        public void TestRandomPopulation()
        {
            using (var provider = new TextCandleProvider())
            {
                provider.SetTextParams("data/si-9-17.dat");
                var gen = new GeneticsSelector<TextCandleProvider, BasicAlgorithm>(provider, 5, 30, 50,
                    generationSize: 100);
                var randomPopulation = gen.GenerateRandomPopulation();
                Assert.IsTrue(randomPopulation.Length == 100);
                Assert.IsTrue(randomPopulation.All(o => o.Length == 50));
                foreach (var ind in randomPopulation)
                    foreach (var p in ind)
                    {
                        Assert.IsTrue(p >= 5 && p <= 30);
                    }
                Assert.IsFalse(randomPopulation.First()
                    .SequenceEqual(randomPopulation.Last()));
            }
        }

        [TestMethod]
        public void TestParentsCross()
        {
            using (var provider = new TextCandleProvider())
            {
                var gen = new GeneticsSelector<TextCandleProvider, BasicAlgorithm>(provider, 5, 30, 50);
                float[] child1;
                float[] child2;
                gen.Cross(new float[] { 1, 2, 3 }, new float[] { 4, 5, 6 }, out child1, out child2);
                Assert.IsTrue(child1.SequenceEqual(new float[] { 4, 2, 6 }));
                Assert.IsTrue(child2.SequenceEqual(new float[] { 1, 5, 3 }));
                gen.Cross(new float[] { 1 }, new float[] { 4, 1 }, out child1, out child2);
                Assert.IsNull(child1);
                Assert.IsNull(child2);
            }
        }

        [TestMethod]
        public void TestMutations()
        {
            using (var provider = new TextCandleProvider())
            {
                var gen = new GeneticsSelector<TextCandleProvider, BasicAlgorithm>(provider, 5, 30, 50);
                var mutatedInd = new float[] { 1, 2, 3, 4 };
                gen.Mutate(mutatedInd);
                Assert.IsFalse(mutatedInd.SequenceEqual(new float[] { 1, 2, 3, 4 }));
            }
        }

        [TestMethod]
        public void TestWaitForGeneticsSelection()
        {
            using (var provider = new TextCandleProvider())
            {
                provider.SetTextParams(@"data\si-9-17.dat");
                var gen = new GeneticsSelector<TextCandleProvider, BasicAlgorithm>(provider, 5, 30, 4);
                gen.Select(2);
                gen.Wait();
                var bestResults = gen.GetBestResults();
                Assert.AreEqual(2, gen.PopulationIndex);
            }
        }

    }
}
