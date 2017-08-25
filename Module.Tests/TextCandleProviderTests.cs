using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FortsRobotLib;
using FortsRobotLib.CandleProviders;

namespace Module.Tests
{
    [TestClass]
    public class TextCandleProviderTests
    {
        [TestMethod]
        public void TestTextCandleProviderInitialization()
        {
            using (var provider = new TextCandleProvider())
            {
                Assert.IsTrue(provider.Initialize());
            }    
        }

        [TestMethod]
        public void TestReadCandleForBaseTextCandleProvider()
        {
            using (var provider = new TextCandleProvider())
            {
                // <DATE>;<TIME>;<OPEN>;<HIGH>;<LOW>;<CLOSE>;<VOL>
                string exStr = "01.01.2017;7:7:7;1;2;3;4;5";
                var candle = provider.GetCandle(exStr);
                Assert.AreEqual(candle, new Candle()
                {
                    TimeStamp = new DateTime(2017,1,1,7,7,7),
                    Open = 1,
                    High = 2,
                    Low = 3,
                    Close = 4,
                    Volume = 5
                });
            }
         }

        [TestMethod]
        public void TestTextCandleProviderReadToEOF()
        {
            using (var provider = new TextCandleProvider())
            {
                int num = 0;
                Candle lastCurrent = new Candle();
                while (provider.MoveNext())
                {
                    Assert.IsTrue(provider.Current != lastCurrent);
                    lastCurrent = provider.Current;
                    num++;
                }
                Assert.IsTrue(num > 1);
            }
        }
    }
}
