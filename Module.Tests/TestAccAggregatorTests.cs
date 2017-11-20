using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ForexRobotLib;
using ForexRobotLib.AccAggregator;
using System.Linq;

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

        [TestMethod]
        public void TestAccAggregatorProfits()
        {
            var acc = new TestAccAgregator();
            var candle = new Candle()
            {
                Close = 1
            };
            acc.Buy(5, candle);
            candle.Close = 10;
            acc.Close(candle);
            Assert.AreEqual(45, acc.Profits[0]);
            candle.Close = 5;
            acc.Sell(1, candle);
            candle.Close = 3;
            acc.Pass(candle);
            candle.Close = 2;
            acc.Sell(2, candle);
            Assert.AreEqual(3, acc.Profits[1]);
            candle.Close = 3;
            acc.Buy(3, candle);
            Assert.AreEqual(-3, acc.Profits[2]);
            Assert.AreEqual(15, acc.MeanProfit);
            Assert.AreEqual((float)2 / 3, acc.SuccessRatio);
            Assert.AreEqual(-3, acc.MeanNegativeProfit);
            Assert.AreEqual(24, acc.MeanPositiveProfit);
        }

        [TestMethod]
        public void TestAccAggregatorSharpIndex()
        {
            var acc = new TestAccAgregator();
            var candle = new Candle()
            {
                TimeStamp = new DateTime(2017, 1, 2, 15, 0, 0),
                Close = 1
            };
            acc.Pass(candle);
            Assert.AreEqual(0, acc.Balance);
            Assert.AreEqual(0, acc.SharpIndex);
            candle.TimeStamp = new DateTime(2017, 2, 1, 15, 0, 0);
            acc.Buy(1, candle);
            Assert.AreEqual(0, acc.SharpIndex);
            candle.TimeStamp = new DateTime(2017, 2, 2, 15, 0, 0);
            candle.Close = 10;
            acc.Sell(2, candle);
            candle.TimeStamp = new DateTime(2017, 3, 2, 15, 0, 0);
            candle.Close = 12;
            acc.Buy(1, candle);
            candle.TimeStamp = new DateTime(2017, 4, 1, 14, 0, 0);
            candle.Close = 15;
            acc.Close(candle);
            Assert.AreEqual(0.5, acc.SuccessRatio);
            Assert.IsTrue(Math.Abs( 3.5 / (float)Math.Sqrt((3.5 - 9)* (3.5 - 9) + (3.5 + 2) * (3.5 + 2)) -  acc.SharpIndex) < 10e-3);
        }
    }
}
