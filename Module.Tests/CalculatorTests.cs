using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ForexRobotLib.CandleProviders;
using BasicAlgorithms;
using System.Linq;
using System.IO;
using ForexRobotLib.AccAggregator;
using ForexRobotLib.Algorithms;
using ForexRobotLib.Calculator;
using ForexRobotLib.ProviderDataCache;
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
            var calc = new Calculator<TextCandleProvider, GuppiAlgorithm>(p, 2);
            IAlgorithm alg;
            var result = calc.Calculate(new float[] { 12, 30, 33, 34, 36, 44 }, out alg);
            Assert.AreEqual(result.Balance, -2383);
            Assert.AreEqual(result.Assets, 0);
            Assert.IsTrue(alg.Data.Length > 0);
        }

        [TestMethod]
        public void TestParallelCalculations()
        {
            var p = new TextCandleProvider();
            p.SetTextParams(@"data\si-9-17.dat");
            var calc = new Calculator<TextCandleProvider, GuppiAlgorithm>(p,4);
            calc.AddParamsForCalculation(new float[] { 13, 12, 11, 10, 9, 8, 7, 6, 5 });
            calc.AddParamsForCalculation(new float[] { 5, 5 });
            // just pass
            calc.Wait();
            calc.CalculateAsync();
            calc.Wait();
            Assert.AreEqual(2, calc.Results.Length);
            Assert.AreNotEqual(calc.Results.First().Balance, calc.Results.Last().Balance);
            calc.Reset();
            calc.AddParamsForCalculation(new float[] { 10, 20, 30});
            calc.AddParamsForCalculation(new float[] { 5,5 });
            calc.CalculateAsync();
            calc.Wait();
            Assert.AreEqual(2, calc.Results.Length);
        }
    }
}
