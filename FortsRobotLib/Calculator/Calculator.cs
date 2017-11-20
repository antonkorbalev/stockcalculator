using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForexRobotLib.AccAggregator;
using ForexRobotLib.Algorithms;
using ForexRobotLib.ProviderDataCache;
using ForexRobotLib.CandleProviders;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace ForexRobotLib.Calculator
{
    public class Calculator<T, T1> : ICalculator<T>, IDisposable
        where T : ICandleProvider
        where T1 : IAlgorithm, new()
    {
        private ManualResetEvent _wait = new ManualResetEvent(true); 
        private MemoryCache<T> _cache;
        private int _threadsNum = 1;
        private ConcurrentQueue<float[]> _ins = new ConcurrentQueue<float[]>();
        private List<CalculationResult> _outs = new List<CalculationResult>();
        private bool _isRunning;
        private CancellationTokenSource _cts;

        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            set
            {
                if (value)
                    _wait.Reset();
                else
                    _wait.Set();
                _isRunning = value;
            }
        }

        public int ThreadsNum
        {
            get
            {
                return _threadsNum;
            }
        }
        public CalculationResult[] Results
        {
            get
            {
                return _outs.ToArray();
            }
        }

        public MemoryCache<T> MemCache
        {
            get
            {
                return _cache;
            }
            set
            {
                _cache = value;
            }
        }

        public void AddParamsForCalculation(float[] parameters)
        {
            _ins.Enqueue(parameters);    
        }

        public void Wait()
        {
            _wait.WaitOne();
        }

        public void Reset()
        {
            _outs.Clear();
            _isRunning = false;
            _cts.Cancel();
            _wait.Set();

        }

        public async Task CalculateAsync()
        {
            IsRunning = true;
            _cts = new CancellationTokenSource();
            await Task.Run(() => 
            {
                while (_ins.Any())
                {
                    try
                    {
                        Parallel.For(0, _threadsNum, (i) =>
                        {
                            float[] parameters;
                            if (_ins.TryDequeue(out parameters))
                            {
                                IAlgorithm alg;
                                var result = Calculate(parameters, out alg);
                                _outs.Add(new CalculationResult()
                                {
                                    Parameters = parameters,
                                    Balance = result.Balance,
                                    SharpIndex = result.SharpIndex,
                                    MeanProfit = result.MeanProfit,
                                    SuccessRatio = result.SuccessRatio,
                                    MeanNegativeProfit = result.MeanNegativeProfit,
                                    MeanPositiveProfit = result.MeanPositiveProfit,
                                    Data = result.Data,
                                    AlgorithmData =  alg.Data 
                                });
                            }
                        });
                    }
                    catch (AggregateException ex)
                    {
                        Trace.Fail(ex.InnerException.Message);    
                    }
                    _cts.Token.ThrowIfCancellationRequested();
                }
                IsRunning = false;
            });    
        }

        public Calculator(MemoryCache<T> cache, int threadsNum = 1) 
        {
            _cache = cache;
            _threadsNum = threadsNum;
            _cts = new CancellationTokenSource();
        }

        public Calculator(T provider, int threadsNum = 1)
        {
            _cache = new MemoryCache<T>(provider);
            _threadsNum = threadsNum;
        }

        public Calculator() { }

        public TestAccAgregator Calculate(float[] parameters, out IAlgorithm alg)
        {
            using (var mc = new MemoryCache<T>(_cache))
            {
                alg = (IAlgorithm)Activator.CreateInstance(typeof(T1));
                var acc = new TestAccAgregator();
                alg.Initialize(parameters);
                Candle curr = mc.Current; 
                while (mc.MoveNext())
                {
                    var answer = alg.Check(mc.Current);
                    if (answer == AlgResult.Buy)
                        acc.Buy(1 - acc.Assets, mc.Current);
                    if (answer == AlgResult.Sell)
                        acc.Sell(1 + acc.Assets, mc.Current);
                    if (answer == AlgResult.Exit)
                        acc.Close(mc.Current);
                    curr = mc.Current;
                }
                acc.Close(curr);
                return acc;
            }

        }

        public void Dispose()
        {
            _cache.Dispose();
        }
    }
}
