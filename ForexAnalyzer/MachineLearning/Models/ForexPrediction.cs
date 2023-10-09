using Microsoft.ML.Data;

namespace ForexAnalyzer.MachineLearning.Models
{
    public class ForexPrediction
    {
        [VectorType(10)]
        [ColumnName("ForexPrediction")]
        public float PredictedClose;
    }
}
