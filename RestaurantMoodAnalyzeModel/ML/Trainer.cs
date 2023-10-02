using Microsoft.Extensions.Configuration;
using Microsoft.ML;
using Microsoft.ML.Calibrators;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using RestaurantMoodAnalyzeModel.Interfaces;
using RestaurantMoodAnalyzeModel.ML.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantMoodAnalyzeModel.ML
{

    /// <summary>
    /// This class trains the model
    /// </summary>
    internal class Trainer : ITrainer
    {
        private readonly IConfiguration _configuration;
        private readonly MLContext _mlContext;


        public Trainer(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        //readonly string filepath = @"C:\Dev\H5\Machine-Learning\RestaurantMoodAnalyzeModel\Data\sampledata.csv";
        //string modelPath = @"C:\Dev\H5\Machine-Learning\RestaurantMoodAnalyzeModel\Data\RestaurantMoodAnalyzeModel.zip";


        /// <summary>
        /// This method trains the model and calls other methods to evaluate and save the model
        /// </summary>
 
        public void Train()
        {
            string? filePath = _configuration["FilePaths:filePath"];
            string? modelPath = _configuration["FilePaths:modelPath"];
            if (!IsFileExists(filePath)) //TODO fix possible null
            {
                Console.WriteLine($"File {filePath} not found");
                return;
            }

            IDataView trainingDataView = LoadData(_mlContext, filePath);
            var dataSplit = SplitData(_mlContext, trainingDataView);
            var dataProcessPipeline = BuildPipeline(_mlContext);
            var trainedModel = TrainModel(dataProcessPipeline, dataSplit.TrainSet);
            SaveModel(_mlContext, trainedModel, dataSplit.TrainSet.Schema, modelPath);   //TODO fix possible null
            EvaluateModel(_mlContext, trainedModel, dataSplit.TestSet);
        }



        /// <summary>
        /// This method checks if the file exists
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Returns true if the file exists</returns>
        private bool IsFileExists(string path)
        {
            return File.Exists(path);
        }



        /// <summary>
        /// This method loads the data from the text file
        /// </summary>
        /// <param name="mLContext"></param>
        /// <param name="filepath"></param>
        /// <returns>Returns the data</returns>
        private IDataView LoadData(MLContext mLContext, string filepath)
        {
            return mLContext.Data.LoadFromTextFile<RestaurantFeedback>(filepath, hasHeader: false, separatorChar: '\t');
        }



        /// <summary>
        /// This method splits the data into training and testing sets
        /// </summary>
        /// <param name="mLContext"></param>
        /// <param name="data"></param>
        /// <returns>Returns the split data</returns>
        private DataOperationsCatalog.TrainTestData SplitData(MLContext mLContext, IDataView data)
        {
            return mLContext.Data.TrainTestSplit(data, testFraction: 0.2);
        }



        /// <summary>
        /// This method builds the data processing pipeline
        /// </summary>
        /// <param name="mLContext"></param>
        /// <returns>Returns the data processing pipeline</returns>
        private EstimatorChain<BinaryPredictionTransformer<CalibratedModelParametersBase<LinearBinaryModelParameters, PlattCalibrator>>> BuildPipeline(MLContext mLContext)
        {
            return mLContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(RestaurantFeedback.Text))
              .Append(mLContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: nameof(RestaurantFeedback.Label), featureColumnName: "Features"));
        }



        /// <summary>
        /// This method trains the model
        /// </summary>
        /// <param name="pipeline"></param>
        /// <param name="trainSet"></param>
        /// <returns>Returns the trained model</returns>
        private ITransformer TrainModel(EstimatorChain<BinaryPredictionTransformer<CalibratedModelParametersBase<LinearBinaryModelParameters, PlattCalibrator>>> pipeline, IDataView trainSet)
        {
            return pipeline.Fit(trainSet);
        }



        /// <summary>
        /// This method saves the trained model
        /// </summary>
        /// <param name="mLContext"></param>
        /// <param name="model"></param>
        /// <param name="schema"></param>
        /// <param name="modelPath"></param>
        private void SaveModel(MLContext mLContext, ITransformer model, DataViewSchema schema, string modelPath)
        {
            mLContext.Model.Save(model, schema, modelPath);
        }



        /// <summary>
        /// This method evaluates the trained model
        /// </summary>
        /// <param name="mLContext"></param>
        /// <param name="model"></param>
        /// <param name="testSet"></param>
        private void EvaluateModel(MLContext mLContext, ITransformer model, IDataView testSet)
        {
            // Evaluate the trained model
            var testSetTransform = model.Transform(testSet);
            var modelMetrics = mLContext.BinaryClassification.Evaluate(data: testSetTransform, labelColumnName: nameof(RestaurantFeedback.Label));
            DisplayMetrics(modelMetrics);
        }



        /// <summary>
        /// Display metrics from trained model
        /// </summary>
        /// <param name="metrics"></param>
        private void DisplayMetrics(CalibratedBinaryClassificationMetrics metrics)
        {
            Console.WriteLine($"Area Under Curve: {metrics.AreaUnderRocCurve:P2}{Environment.NewLine}" +
                $"Area Under Precision Recall Curve: {metrics.AreaUnderPrecisionRecallCurve:P2}{Environment.NewLine}" +
                $"Accuracy: {metrics.Accuracy:P2}{Environment.NewLine}" +
                $"F1Score: {metrics.F1Score:P2}{Environment.NewLine}" +
                $"Positive Recall: {metrics.PositiveRecall:#.##}{Environment.NewLine}" +
                $"Negative Recall: {metrics.NegativeRecall:#.##}{Environment.NewLine}");
        }
    }
}
