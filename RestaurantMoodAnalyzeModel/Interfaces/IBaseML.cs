using Microsoft.ML;

namespace RestaurantMoodAnalyzeModel.Interfaces
{
    internal interface IBaseML
    {
        public MLContext GetContext();
    }
}