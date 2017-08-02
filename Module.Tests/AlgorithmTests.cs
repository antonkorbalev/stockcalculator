using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FortsRobotLib.CandleProviders;

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
                provider.SetTextParams("data/si-9-17.dat", ';');
                while (provider.MoveNext() && provider.Current.TimeStamp != new DateTime(2017, 6, 8, 11, 0, 0))
                {
                }
                Assert.IsTrue(provider.Current.Close == 58204);

            }
        }
    }
}
