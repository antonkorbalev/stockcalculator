using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FortsRobotLib.CandleProviders;
using FortsRobotLib.Algorithms;
using FortsRobotLib.Calculator;
using System.Diagnostics;
using System.Threading;
using FortsRobotLib.ProviderDataCache;

namespace FortsRobotLib.Genetics
{
    public class GeneticsSelector<T, T1> : IGeneticsSelector
        where T : ICandleProvider
        where T1 : IAlgorithm, new()
    {
        private int _mutationsPercent;
        private int _generationSize;
        private int _selectionPercent;
        private int _lowParamBorder;
        private int _highParamBorder;
        private int _paramsCount;
        private int _maxGeneration;
        private int _crossPercent;
        private Random _rand = new Random();
        private Calculator<T, T1> _calculator;
        private Func<CalculationResult, float> _selectCondition;
        private bool _isRunning;
        private CancellationTokenSource _cts;
        private ManualResetEvent _wait;
        IAlgorithm _alg;

        public event EventHandler<PopulationCompletedEventArgs> PopulationCompleted;
        public int PopulationIndex { get; private set; }

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

        public void Wait()
        {
            _wait.WaitOne();
        }

        public void Cancel()
        {
            _cts.Cancel();
        }

        private void Initialize(int lowParamBorder,
            int highParamBorder, int paramsCount, int threadsNum,
            int generationSize, int selectionPercent, int mutationsPercent, int crossPercent,
            Func<CalculationResult, float> selectCondition)
        {
            _generationSize = generationSize;
            _selectionPercent = selectionPercent;
            _mutationsPercent = mutationsPercent;
            _lowParamBorder = lowParamBorder;
            _highParamBorder = highParamBorder;
            _paramsCount = paramsCount;
            _crossPercent = crossPercent;

            // select condition for individuals
            if (selectCondition == null)
                _selectCondition = new Func<CalculationResult, float>(o => o.SharpIndex);
            else
                _selectCondition = selectCondition;
            _wait = new ManualResetEvent(true);
            _cts = new CancellationTokenSource();
            _alg = (IAlgorithm)Activator.CreateInstance(typeof(T1));
        }

        public GeneticsSelector(T provider, int lowParamBorder,
            int highParamBorder, int paramsCount, int threadsNum = 4,
            int generationSize = 100, int selectionPercent = 30, int mutationsPercent = 5, int crossPercent = 50,
            Func<CalculationResult, float> selectCondition = null)
        {
            Initialize(lowParamBorder, highParamBorder, paramsCount, threadsNum, generationSize,
                selectionPercent, mutationsPercent, crossPercent,  selectCondition);
            _calculator = new Calculator<T, T1>(provider, threadsNum);
        }

        public GeneticsSelector(MemoryCache<T> cache, int lowParamBorder,
            int highParamBorder, int paramsCount, int threadsNum = 4,
            int generationSize = 100, int selectionPercent = 30, int mutationsPercent = 5, int crossPercent = 50,
            Func<CalculationResult, float> selectCondition = null)
        {
            Initialize(lowParamBorder, highParamBorder, paramsCount, threadsNum, generationSize,
                selectionPercent, mutationsPercent, crossPercent, selectCondition);
            _calculator = new Calculator<T, T1>(cache, threadsNum);
        }

        internal float[][] GenerateRandomPopulation()
        {
            var result = new float[_generationSize][];
            for (var i = 0; i < _generationSize; i++)
            {
                var prms = new float[_paramsCount];
                for (var num = 0; num < _paramsCount; num++)
                    prms[num] = _lowParamBorder + _rand.Next(_highParamBorder - _lowParamBorder);
                result[i] = prms.ToArray();
            }
            return result;
        }

        internal void Cross(float[] parent1, float[] parent2, out float[] child1, out float[] child2)
        {
            child1 = null;
            child2 = null;
            if (parent1.Length != parent2.Length)
                return;

            child1 = new float[parent1.Length];
            child2 = new float[parent2.Length];

            for (var i = 0; i < parent1.Length; i++)
                if (i % 2 == 0)
                {
                    child1[i] = parent2[i];
                    child2[i] = parent1[i];
                }
                else
                {
                    child1[i] = parent1[i];
                    child2[i] = parent2[i];
                }
        }

        internal void Mutate(float[] ind)
        {
            var i1 = _rand.Next(ind.Length);
            var i2 = _rand.Next(ind.Length);
            if (i1 == i2)
            {
                Mutate(ind);
                return;
            }
            ind[i1] = ind[i2] - (ind[i2] = ind[i1]) + ind[i1];
        }

        public CalculationResult[] GetBestResults()
        {
            int aliveCount = _generationSize * _selectionPercent / 100;
            return _calculator.Results.OrderByDescending(_selectCondition).ThenByDescending(o => o.Balance)
                .Take(aliveCount).ToArray();
        }

        internal async Task CalculateNextPopulation()
        {
            await _calculator.CalculateAsync();

            // actions after calculation finishes
            PopulationIndex++;
            var crossInds = GetBestResults();
            var handler = PopulationCompleted;
            if (handler != null)
                handler(this, new PopulationCompletedEventArgs()
                {
                    PopulationIndex = PopulationIndex,
                    Results = crossInds
                });
            if ((PopulationIndex >= _maxGeneration) || _cts.IsCancellationRequested)
            {
                IsRunning = false;
                return;
            }

            // select individuals
            var newGen = new List<float[]>();
            while (newGen.Count() < _generationSize)
            {
                float[] child1, child2;
                var parent1 = crossInds[_rand.Next(crossInds.Length)].Parameters;
                var parent2 = crossInds[_rand.Next(crossInds.Length)].Parameters;
                if (_rand.Next(100) <= _crossPercent)
                    Cross(parent1, parent2, out child1, out child2);
                else
                {
                    child1 = parent1;
                    child2 = parent2;
                }

                newGen.Add(child1);
                newGen.Add(child2);
            }

            // mutate individuals
            int mutateCount = _generationSize * _mutationsPercent / 100;
            for (var i = 0; i < mutateCount; i++)
                Mutate(newGen[_rand.Next(_generationSize)]);

            // add new generation for calculation
            Trace.Assert(Math.Abs(newGen.Count() - _generationSize) <= 1);
            _calculator.Reset();
            foreach (var ind in newGen)
                _calculator.AddParamsForCalculation(ind);

            // calculate
            await CalculateNextPopulation();
        }

        public async void Select(int maxGeneration)
        {
            IsRunning = true;
            _maxGeneration = maxGeneration;

            // create random population
            foreach (var ind in GenerateRandomPopulation())
                _calculator.AddParamsForCalculation(ind);
            await CalculateNextPopulation();
        }
    }
}
