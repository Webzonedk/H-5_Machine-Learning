using ForexAnalyzer.Interfaces;
using ForexAnalyzer.MachineLearning.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForexAnalyzer.Interfaces
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

        public List<ForexPrediction> Predict()
        {
            //var inputData = GetInputPath();
            //if (inputData == null)
            //{
            //    Console.WriteLine("inputData is not configured");
            //    return null;
            //}
            var mlContext = _mlContext.GetContext();
            string? modelPath = GetModelPath();
            if (modelPath == null)
            {
                Console.WriteLine("Model path is not configured");
                return null;
            }

            ITransformer mlModel = LoadModel(mlContext, modelPath);
            if (mlModel == null)
            {
                Console.WriteLine("Failed to load model");
                return null;
            }

            return MakePrediction(mlContext, mlModel);
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



        private List<ForexPrediction> MakePrediction(MLContext mlContext, ITransformer mlModel)
        {
            var forecastEngine = mlModel.CreateTimeSeriesEngine<ForexData, ForexPrediction>(mlContext);
            var forecasts = forecastEngine.Predict(horizon: 3, confidenceLevel: 0.95f);
            var forecastList = new List<ForexPrediction> { forecasts };
            return forecastList;
        }
    }
}
