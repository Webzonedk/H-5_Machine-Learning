using Microsoft.Extensions.Configuration;
using Microsoft.ML;
using RestaurantMoodAnalyzeModel.Interfaces;
using RestaurantMoodAnalyzeModel.ML.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantMoodAnalyzeModel.ML
{
    internal class Predictor : IPredictor
    {

        private readonly MLContext _mlContext;

        private readonly IConfiguration _configuration;

        public Predictor(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        public void Predict(string inputData)
        {
            string? modelPath = _configuration["FilePaths:modelPath"];
            if (!File.Exists(modelPath))
            {
                Console.WriteLine($"\"Failed to find model at {modelPath}");
                return;
            }


            ITransformer mlModel;
            using (var stream = new FileStream(modelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                mlModel = _mlContext.Model.Load(stream, out _);
            }
            if (mlModel == null)
            {
                Console.WriteLine("Failed to load model"); return;
            }

            var predictionEngine = _mlContext.Model.CreatePredictionEngine<RestaurantFeedback, RestaurantPrediction>(mlModel);

            var prediction = predictionEngine.Predict(new RestaurantFeedback { Text = inputData });

            Console.WriteLine($"Based on \"{inputData}\", the feedback is predicted to be:{Environment.NewLine}{(prediction.Prediction ? "Negative" : "Positive")} at a {prediction.Probability:P0} confidence");
        }
    }
}
