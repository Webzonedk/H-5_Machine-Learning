using Microsoft.Extensions.Configuration;
using Microsoft.ML;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkforceTeardown.Interfaces;
using WorkforceTeardown.Interfaces.Models;

namespace WorkforceTeardown
{
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

        public void Predict()
        {
            var inputData = GetInputPath();
            if (inputData == null)
            {
                Console.WriteLine("inputData is not configured");
                return;
            }
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



        private string? GetInputPath()
        {
            return _configuration["FilePaths:filePath"];
        }

        private string? GetModelPath()
        {
            return _configuration["FilePaths:modelPath"];
        }



        private ITransformer LoadModel(MLContext mlContext, string modelPath)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load model: {ex.Message}");
                return null;
            }
        }



        private void MakePrediction(MLContext mlContext, ITransformer mlModel, string inputData)
        {
            if (!File.Exists(inputData))
            {
                Console.WriteLine($"Input file not found at {inputData}");
                return;
            }
            var predictionEngine = mlContext.Model.CreatePredictionEngine<EmploymentHistory, EmploymentHistoryPrediction>(mlModel);
            var json = File.ReadAllText(inputData);
            var employmentHistory = JsonConvert.DeserializeObject<EmploymentHistory>(json);
            if (employmentHistory == null)
            {
                Console.WriteLine("Failed to deserialize input data to EmploymentHistory object");
                return;
            }
            var prediction = predictionEngine.Predict(employmentHistory);

            Console.WriteLine($"Based on the json input file:{System.Environment.NewLine}" +
                $"{json}{System.Environment.NewLine}" + $"The employee is predicted to work {prediction.PredictedMonths:#.##} months");
        }
    }
}
