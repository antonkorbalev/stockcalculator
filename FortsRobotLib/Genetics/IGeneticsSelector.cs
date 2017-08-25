using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FortsRobotLib.Calculator;

namespace FortsRobotLib.Genetics
{
    public interface IGeneticsSelector
    {
        void Select(int maxGeneration);
        event EventHandler<PopulationCompletedEventArgs> PopulationCompleted;
        int PopulationIndex { get; }
        bool IsRunning { get; }
        void Cancel();
        void Wait();
        CalculationResult[] GetBestResults(out int populationIndex);
    }
}
