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
        //private readonly IConfiguration _configuration;
        //private readonly string? modelPath;
        //protected readonly string ModelPath;
        protected readonly MLContext MlContext;

        public BaseML(IConfiguration configuration)
        {
            //_configuration = configuration;
            //modelPath = _configuration["FilePaths:modelPath"];
            //ModelPath = Path.Combine(AppContext.BaseDirectory, modelPath);
            MlContext = new MLContext(2020);
        }
        public MLContext GetContext()
        {
            return MlContext;
        }

    }
}
