using Microsoft.ML;

namespace WorkforceTeardown.Interfaces
{
    internal interface IBaseML
    {
       public  MLContext GetContext();
    }
}