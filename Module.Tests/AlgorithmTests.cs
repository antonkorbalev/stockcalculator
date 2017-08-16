using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FortsRobotLib.CandleProviders;
using BasicAlgorithms;
using System.Linq;
using System.IO;
using FortsRobotLib.AccAggregator;
using FortsRobotLib.Algorithms;

namespace Module.Tests
{
    [TestClass]
    public class AlgorithmTests
    {
        [TestMethod]
        public void TestBasicAlgDataIntegrity()
        {
            using (var provider = new TextCandleProvider())
            {
                provider.SetTextParams("data/si-9-17.dat", ';');
                while (provider.MoveNext())
                {
                    var curr = provider.Current;
                    if (curr.TimeStamp == new DateTime(2017, 7, 13, 13, 0, 0))
                    {
                        Assert.IsTrue(curr.Open == 61036 && curr.Close == 60804 && curr.High == 61049 && curr.Low == 60785);
                    }
                    if (curr.TimeStamp == new DateTime(2017, 6, 14, 17, 0, 0))
                    {
                        Assert.IsTrue(curr.Open == 58120 && curr.Close == 58115 && curr.High == 58126 && curr.Low == 58016);
                    }
                    if (curr.TimeStamp == new DateTime(2017, 7, 14, 19, 0, 0))
                    {
                        Assert.IsTrue(curr.Open == 60076 && curr.Close == 59940 && curr.High == 60109 && curr.Low == 59940);
                    }
                }
                Assert.IsTrue(provider.Current.TimeStamp > new DateTime(2017, 8,2));    
            }
        }

        [TestMethod]
        public void TestBasicAlgMatrix()
        {
            using (var provider = new TextCandleProvider())
            {
                var alg = new BasicAlgorithm(13, 12, 11, 10, 9, 8, 7, 6, 5);
                provider.SetTextParams("data/si-9-17.dat", ';');
                while (provider.MoveNext() && provider.Current.TimeStamp != new DateTime(2017, 6, 1, 15, 0, 0))
                {
                }
                Assert.IsTrue(provider.Current.Close == 58054);
                while (provider.MoveNext() && provider.Current.TimeStamp != new DateTime(2017, 6, 8, 11, 0, 0))
                {
                    alg.Check(provider.Current);
                }
                alg.Check(provider.Current);
                Assert.IsTrue(provider.Current.Close == 58204);
                using (var reader = new StreamReader("../../data/basic_data.dat"))
                {
                    while (provider.MoveNext() && provider.Current.TimeStamp != new DateTime(2017, 6, 10, 0, 0, 0))
                    {
                        var answer = alg.Check(provider.Current);
                        var data = alg.Data.Last();
                        var values = reader.ReadLine().Split(';').ToArray();
                        for (var i = 3; i < 12; i++)
                        {
                            Assert.AreEqual(Math.Truncate((data[i-2])), Math.Truncate(float.Parse(values[i])));
                        }
                        Assert.AreEqual(Math.Truncate(float.Parse(values[2])), Math.Truncate(data[0]));
                        System.Diagnostics.Trace.WriteLine(answer);
                    }
                }
                Assert.IsTrue(provider.Current.Close == 58220);
            }
        }

        [TestMethod]
        public void TestBasicAlgProfit()
        {
            using (var provider = new TextCandleProvider())
            {
                provider.SetTextParams("data/si-9-17.dat", ';');
                var alg = new BasicAlgorithm(13, 12, 11, 10, 9, 8, 7, 6, 5);
                var acc = new TestAccAgregator();
                while (provider.MoveNext())
                {
                    var answer = alg.Check(provider.Current);
                    if (answer == FortsRobotLib.AlgResult.Buy)
                        acc.Buy(1 - acc.Assets,provider.Current);
                    if (answer == FortsRobotLib.AlgResult.Sell)
                        acc.Sell(1 + acc.Assets, provider.Current);
                    if (answer == FortsRobotLib.AlgResult.Exit)
                        acc.Close(provider.Current);
                }
                acc.Close(provider.Current);
                Assert.IsTrue(acc.Balance > 0);
            }
        }

        [TestMethod]
        public void TestAlgorithmBaseInit()
        {
            var alg = new AlgorithmBase();
            alg.Initialize(new[] { 1f });
            Assert.AreEqual(alg.Parameters.Length, 1);
            alg.Initialize(new[] { 1f, 2f, 3f });
            Assert.AreEqual(alg.Parameters.Length, 1);
        }
    }
}
