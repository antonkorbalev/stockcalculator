using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FortsRobotLib.CandleProviders;
using FortsRobotLib;
using System.Linq;
using System.IO;

namespace Module.Tests
{
    [TestClass]
    public class FinamCandleProviderTests
    {
        [TestMethod]
        public void TestFinamProviderBasicDownload()
        {
            var lowDate = new DateTime(2016, 6, 10);
            var highDate = new DateTime(2016, 7, 10);
            using (var provider = new FinamCandleProvider("SPFB.SI", TimePeriod.Hour,
                "14", "19899", lowDate, highDate))
            {
                Assert.AreEqual(provider.Current, default(Candle));
                provider.Initialize();
                var num = 0;
                while (provider.MoveNext())
                {
                    num++;
                    Assert.IsTrue(provider.Current.TimeStamp >= lowDate && provider.Current.TimeStamp <= highDate);
                }
                Assert.AreEqual(279, num);     
            }
        }
    }
}
