using Microsoft.Extensions.Configuration;
using Microsoft.ML;
using RestaurantMoodAnalyzeModel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantMoodAnalyzeModel.ML.Base
{
    internal class BaseML : IBaseML
    {
        protected readonly MLContext MlContext;

        public BaseML(IConfiguration configuration)
        {
            MlContext = new MLContext(2020);
        }
        public MLContext GetContext()
        {
            return MlContext;
        }
    }
}
