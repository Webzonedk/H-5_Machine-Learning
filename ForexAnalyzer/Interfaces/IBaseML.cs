using Microsoft.ML;

namespace ForexAnalyzer.Interfaces
{
    internal interface IBaseML
    {
        MLContext GetContext();
    }
}