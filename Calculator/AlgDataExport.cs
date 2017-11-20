using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForexRobotLib.Calculator;
using System.IO;
using Calculator.Properties;

namespace Calculator
{
    internal static class AlgDataExport
    {
        internal static void ExportToCSV(CalculationResult result, string fileName, string info)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
            File.AppendAllText(fileName, string.Format("Algorithm: {0}{2}Parameters: {1}{2};<INSTRUMENT>;<TIME>;<OPEN>;<HIGH>;<LOW>;<CLOSE>;<AMOUNT>;<BALANCE>;<RESULT>;;{3}{2}",
                info, string.Join(",", result.Parameters), Environment.NewLine, result.AlgorithmData.Any() ? "<ALGORITHM DATA>" : string.Empty));
            var lines = result.Data.Select(o =>
                string.Format(";{0};{1};{2};{3};{4};{5};{6};{7};{8};",
                    Settings.Default.InsName,
                    o.Candle.TimeStamp,
                    o.Candle.Open,
                    o.Candle.High,
                    o.Candle.Low,
                    o.Candle.Close,
                    o.Amount,
                    o.Balance,
                    o.Amount > 0 ?
                        "LONG" : (o.Amount < 0 ?
                            "SHORT" : "EXIT")
                    )
                ).Take(result.Data.Length - 1).ToArray();

            for (int li = 0; li < result.AlgorithmData.Length - 1; li++)
            {
                lines[lines.Length - li - 1] = string.Format("{0};{1};",
                    lines[lines.Length - li - 1],
                    string.Join(";", result.AlgorithmData[result.AlgorithmData.Length - li - 1]));
            }
            File.AppendAllLines(fileName, lines);
        }
    }
}
