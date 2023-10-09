using ForexAnalyzer.MachineLearning.Models;

namespace ForexAnalyzer.Interfaces
{
    public interface IPredictor
    {
        public List<ForexPrediction> Predict();
    }
}