using Microsoft.Extensions.Configuration;
using Microsoft.ML;
using RestaurantMoodAnalyzeModel.Interfaces;
using RestaurantMoodAnalyzeModel.ML.Objects;
using System;
using System.IO;

namespace RestaurantMoodAnalyzeModel.ML
{
    /// <summary>
    /// This
    /// </summary>
    internal class Predictor : IPredictor
    {
        private readonly IConfiguration _configuration;
        private readonly IBaseML _mlContext;

        public Predictor(
            IConfiguration configuration,
            IBaseML mlContext
            )
        {
            _configuration = configuration;
            _mlContext = mlContext;
        }

        public void Predict(string inputData)
        {
            var mlContext = _mlContext.GetContext();
            string? modelPath = GetModelPath();
            if (modelPath == null)
            {
                Console.WriteLine("Model path is not configured");
                return;
            }

            ITransformer mlModel = LoadModel(mlContext, modelPath);
            if (mlModel == null)
            {
                Console.WriteLine("Failed to load model");
                return;
            }

            MakePrediction(mlContext, mlModel, inputData);
        }

        private string? GetModelPath()
        {
            return _configuration["FilePaths:modelPath"];
        }

        private ITransformer LoadModel(MLContext mlContext, string modelPath)
        {
            if (!File.Exists(modelPath))
            {
                Console.WriteLine($"Failed to find model at {modelPath}");
                return null;
            }

            using (var stream = new FileStream(modelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return mlContext.Model.Load(stream, out _);
            }
        }

        private void MakePrediction(MLContext mlContext, ITransformer mlModel, string inputData)
        {
            var predictionEngine = mlContext.Model.CreatePredictionEngine<RestaurantFeedback, RestaurantPrediction>(mlModel);

            var prediction = predictionEngine.Predict(new RestaurantFeedback { Text = inputData });

            Console.WriteLine($"Based on \"{inputData}\", the feedback is predicted to be:{Environment.NewLine}{(prediction.Prediction ? "Negative" : "Positive")} at a {prediction.Probability:P0} confidence");
        }
    }
}
