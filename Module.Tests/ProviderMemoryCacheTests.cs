using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ForexRobotLib;
using ForexRobotLib.ProviderDataCache;
using ForexRobotLib.CandleProviders;
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
            using (var p1 = new TextCandleProvider())
            {
                p1.SetTextParams(@"data\si-9-17.dat");
                Assert.IsTrue(p1.Initialize());
                candles.Add(p1.Current);
                while (p1.MoveNext())
                    candles.Add(p1.Current);
            }
            var p = new TextCandleProvider();
            p.SetTextParams(@"data\si-9-17.dat");
            // p gets automatically disposed
            using (var mc = new MemoryCache<TextCandleProvider>(p))
            {
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

        [TestMethod]
        public void TestCopyMemoryCache()
        {
            var p = new TextCandleProvider();
            p.SetTextParams(@"data\si-9-17.dat");
            using (var mc = new MemoryCache<TextCandleProvider>(p))
            {
                mc.MoveNext();
                using (var mc1 = new MemoryCache<TextCandleProvider>(mc))
                {
                    Assert.IsTrue(mc1.Current == default(Candle));
                    mc1.MoveNext();
                    Assert.AreEqual(mc.Current, mc1.Current);
                    mc1.MoveNext();
                    Assert.AreNotEqual(mc.Current, mc1.Current);
                    mc.MoveNext();
                    Assert.AreEqual(mc.Current, mc1.Current);
                }
            }
        }
    }
}
