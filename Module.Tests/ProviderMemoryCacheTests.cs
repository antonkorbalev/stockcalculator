using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FortsRobotLib;
using FortsRobotLib.ProviderDataCache;
using FortsRobotLib.CandleProviders;
using System.Collections.Generic;

namespace Module.Tests
{
    [TestClass]
    public class ProviderMemoryCacheTests
    {
        [TestMethod]
        public void TestProviderMemoryCache()
        {
            var candles = new List<Candle>();
            using (var p = new TextCandleProvider())
            {
                p.SetTextParams(@"data\si-9-17.dat");
                p.Initialize();
                candles.Add(p.Current);
                while (p.MoveNext())
                    candles.Add(p.Current);
            }
            using (var mc = new MemoryCache<TextCandleProvider>())
            {
                var p = new TextCandleProvider();
                p.SetTextParams(@"data\si-9-17.dat");
                mc.Initialize(p);
                var num = 0;
                while (mc.MoveNext())
                {
                    Assert.AreEqual(candles[num], mc.Current);
                    num++;
                }
                mc.Reset();
                mc.MoveNext();
                Assert.AreEqual(candles[0], mc.Current);
            }

        }
    }
}
