using Microsoft.ML;

namespace RestaurantMoodAnalyzeModel.Interfaces
{
    internal interface IBaseML
    {
        MLContext GetContext();
    }
}