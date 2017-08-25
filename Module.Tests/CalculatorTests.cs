using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FortsRobotLib.CandleProviders;
using BasicAlgorithms;
using System.Linq;
using System.IO;
using FortsRobotLib.AccAggregator;
using FortsRobotLib.Algorithms;
using FortsRobotLib.Calculator;
using FortsRobotLib.ProviderDataCache;
using System.Threading;

namespace Module.Tests
{
    [TestClass]
    public class CalculatorTests
    {
        [TestMethod]
        public void TestSingleCalculation()
        {
            var p = new TextCandleProvider();
            p.SetTextParams(@"data\si-9-17.dat");
            var calc = new Calculator<TextCandleProvider, BasicAlgorithm>(p, 2);
            var result = calc.Calculate(new float[] { 13, 12, 11, 10, 9, 8, 7, 6, 5, 5, 6, 7, 8,9, 10, 11, 12, 13 });
            Assert.AreEqual(result.Balance, 2379);
            Assert.AreEqual(result.Assets, 0);
        }

        [TestMethod]
        public void TestParallelCalculations()
        {
            var p = new TextCandleProvider();
            p.SetTextParams(@"data\si-9-17.dat");
            var calc = new Calculator<TextCandleProvider, BasicAlgorithm>(p,4);
            calc.AddParamsForCalculation(new float[] { 13, 12, 11, 10, 9, 8, 7, 6, 5 });
            calc.AddParamsForCalculation(new float[] { 5, 5 });
            // just pass
            calc.Wait();
            calc.CalculateAsync();
            calc.Wait();
            Assert.AreEqual(2, calc.Results.Length);
            Assert.AreNotEqual(calc.Results.First().Profit, calc.Results.Last().Profit);
            calc.Reset();
            calc.AddParamsForCalculation(new float[] { 10, 20, 30});
            calc.AddParamsForCalculation(new float[] { 5,5 });
            calc.CalculateAsync();
            calc.Wait();
            Assert.AreEqual(2, calc.Results.Length);
        }
    }
}
