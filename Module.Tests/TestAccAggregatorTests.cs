using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FortsRobotLib;
using FortsRobotLib.AccAggregator;

namespace Module.Tests
{
    [TestClass]
    public class TestAccAggregatorTests
    {
        [TestMethod]
        public void TestAccAggregatorBuyAndHold()
        {
            var acc = new TestAccAgregator();
            var candle = new Candle()
            {
                Close = 1
            };
            acc.Pass(candle);
            Assert.AreEqual(acc.Balance, 0);
            acc.Buy(1, candle);
            Assert.AreEqual(acc.Balance, 0);
            candle.Close = 2;
            acc.Pass(candle);
            Assert.AreEqual(acc.Balance, 1);
            candle.Close = 0;
            acc.Pass(candle);
            Assert.AreEqual(acc.Balance, -1);
            candle.Close = 1;
            acc.Close(candle);
            Assert.AreEqual(acc.Balance, 0);
            Assert.AreEqual(acc.Data.Length, 5);
        }

        [TestMethod]
        public void TestAccAggregatorBuySell()
        {
            var acc = new TestAccAgregator();
            var candle = new Candle()
            {
                Close = 1
            };
            acc.Buy(1, candle);
            candle.Close = 0;
            acc.Sell(1, candle);
            Assert.AreEqual(acc.Balance, -1);
            Assert.AreEqual(acc.Assets, 0);
        }

        [TestMethod]
        public void TestAccAggregatorReset()
        {
            var acc = new TestAccAgregator();
            var candle = new Candle()
            {
                Close = 1
            };
            acc.Buy(1, candle);
            Assert.AreEqual(acc.Assets, 1);
            acc.Reset();
            Assert.AreEqual(acc.Balance, 0);
            Assert.AreEqual(acc.Data.Length, 0);
            Assert.AreEqual(acc.Assets, 0);
        }

        [TestMethod]
        public void TestAccAggregatorBuySellClose()
        {
            var acc = new TestAccAgregator();
            var candle = new Candle()
            {
                Close = 1
            };
            acc.Buy(5, candle);
            candle.Close = 10;
            acc.Pass(candle);
            candle.Close = 7;
            acc.Sell(3, candle);
            candle.Close = 5;
            acc.Pass(candle);
            candle.Close = 1;
            acc.Sell(10, candle);
            acc.Close(candle);
            Assert.AreEqual(acc.Balance, 18);
            Assert.AreEqual(acc.Data.Length, 6);
            Assert.AreEqual(acc.Assets, 0);
        }
    }
}
