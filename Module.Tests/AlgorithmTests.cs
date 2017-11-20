using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ForexRobotLib.CandleProviders;
using BasicAlgorithms;
using System.Linq;
using System.IO;
using ForexRobotLib.AccAggregator;
using ForexRobotLib.Algorithms;
using ForexRobotLib;
using System.Globalization;

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
                Assert.IsTrue(provider.Current.TimeStamp > new DateTime(2017, 8, 2));
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
            alg.Reset();
            alg.Initialize(new[] { 1f });
            Assert.AreEqual(alg.Parameters.Length, 1);
        }

        [TestMethod]
        public void TestGuppiAlgData()
        {
            using (var provider = new TextCandleProvider())
            {
                provider.SetTextParams(@"data\si-9-17.dat", ';');
                var alg = new GuppiAlgorithm(4, 6, 9, 13, 31, 36, 41, 46, 51, 61);
                var acc = new TestAccAgregator();
                for (var i = 0; i < 61; i++)
                {
                    provider.MoveNext();
                    var res = alg.Check(provider.Current);
                    Assert.IsTrue(res == AlgResult.Exit && !alg.Data.Last().Any());
                }
                while (provider.MoveNext())
                {
                    var answer = alg.Check(provider.Current);
                    if (answer == ForexRobotLib.AlgResult.Buy)
                        acc.Buy(1 - acc.Assets, provider.Current);
                    if (answer == ForexRobotLib.AlgResult.Sell)
                        acc.Sell(1 + acc.Assets, provider.Current);
                    if (answer == ForexRobotLib.AlgResult.Exit)
                        acc.Close(provider.Current);
                    Assert.IsTrue(alg.Data.Last().Length == 10 + 10 + 8 + 3);
                }
            }
        }

        [TestMethod]
        public void TestGuppiAlgMatrix()
        {
            var alg = new GuppiAlgorithm(12, 30, 33, 34, 36, 44);
            using (var provider = new FinamCandleProvider("SPFB.SI", TimePeriod.Hour,
                "14", "19899", new DateTime(2015, 5, 1), new DateTime(2015, 10, 1)))
            {
                provider.Initialize();
                using (var reader = new StreamReader(@"data\guppi_data.dat"))
                {
                    var startDate = new DateTime(2015, 5, 15, 0, 0, 0);
                    while (provider.Current.TimeStamp != startDate)
                    {
                        provider.MoveNext();
                        alg.Check(provider.Current);
                    }
                    reader.ReadLine();
                    while (!reader.EndOfStream)
                    {
                        provider.MoveNext();
                        alg.Check(provider.Current);
                        var data = reader.ReadLine().Split(';');
                        var algData = alg.Data.Last();
                        for (var i = 4; i < 10; i++)
                        {
                            var mean = float.Parse(data[i], CultureInfo.InvariantCulture);
                            Assert.IsTrue(Math.Abs(algData[i - 4] - mean) < 0.01);
                        }
                        for (var i = 11; i <= 22; i++)
                        {
                            var sign = float.Parse(data[i]);
                            Assert.AreEqual(sign, algData[i - 5]);
                        }
                    }
                }
            }       
        }
    }
}
