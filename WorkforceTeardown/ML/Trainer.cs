using Microsoft.Extensions.Configuration;
using Microsoft.ML.Calibrators;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkforceTeardown.Interfaces;
using WorkforceTeardown.Interfaces.Models;
using WorkforceTeardown.Tools;

namespace WorkforceTeardown.Interfaces
{
    /// <summary>
    /// This class trains the model
    /// </summary>
    internal class Trainer : ITrainer
    {
        private readonly IConfiguration _configuration;
        private readonly IBaseML _mlContext;


        public Trainer(
            IConfiguration configuration,
            IBaseML baseML
            )
        {
            _configuration = configuration;
            _mlContext = baseML;
        }

        /// <summary>
        /// This method trains the model and calls other methods to evaluate and save the model
        /// </summary>

        public void Train()
        {
            var mlContext = _mlContext.GetContext();
            string? modelPath = _configuration["FilePaths:modelPath"];
            string? trainingPath = _configuration["FilePaths:TrainingFilePath"];
            if (!IsFileExists(trainingPath!)) 
            {
                Console.WriteLine($"File {trainingPath} not found");
                return;
            }

            //if (!IsFileExists(modelPath!))
            //{
            //    Console.WriteLine($"File {modelPath} not found");
            //    return;
            //}

            IDataView trainingDataView = LoadData(mlContext, trainingPath!);
            var dataSplit = SplitData(mlContext, trainingDataView);
            var dataProcessPipeline = BuildPipeline(mlContext);
            var trainedModel = TrainModel(dataProcessPipeline, dataSplit.TrainSet);
            SaveModel(mlContext, trainedModel, dataSplit.TrainSet.Schema, modelPath!);
            EvaluateModel(mlContext, trainedModel, dataSplit.TestSet);
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
        private IDataView LoadData(MLContext mLContext, string trainingPath)
        {
            var data = mLContext.Data.LoadFromTextFile<EmploymentHistory>(trainingPath, separatorChar: ',');
            Console.WriteLine($"Loaded {data.GetRowCount()} records from {trainingPath}");
            //PreviewData(mLContext, data); //Previewing the data in the sample data for training.
            return data;
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
        private EstimatorChain<RegressionPredictionTransformer<LinearRegressionModelParameters>> BuildPipeline(MLContext mLContext)
        {
            var dataProcessingPipeline = mLContext.Transforms.CopyColumns("Label", nameof(EmploymentHistory.DurationInMonths))
                  .Append(mLContext.Transforms.NormalizeMeanVariance(nameof(EmploymentHistory.IsMarried)))
                  .Append(mLContext.Transforms.NormalizeMeanVariance(nameof(EmploymentHistory.BsDegree)))
                  .Append(mLContext.Transforms.NormalizeMeanVariance(nameof(EmploymentHistory.MsDegree)))
                  .Append(mLContext.Transforms.NormalizeMeanVariance(nameof(EmploymentHistory.YearsExperience)))
                  .Append(mLContext.Transforms.NormalizeMeanVariance(nameof(EmploymentHistory.AeAtHire)))
                  .Append(mLContext.Transforms.NormalizeMeanVariance(nameof(EmploymentHistory.HasKids)))
                  .Append(mLContext.Transforms.NormalizeMeanVariance(nameof(EmploymentHistory.WithinMonthOfVesting)))
                  .Append(mLContext.Transforms.NormalizeMeanVariance(nameof(EmploymentHistory.DeskDecorations)))
                  .Append(mLContext.Transforms.NormalizeMeanVariance(nameof(EmploymentHistory.LongCommute)))
                  .Append(mLContext.Transforms.Concatenate("Features", typeof(EmploymentHistory).ToPropertyList<EmploymentHistory>(nameof(EmploymentHistory.DurationInMonths))));

            var trainingPipeline = dataProcessingPipeline.Append(
                mLContext.Regression.Trainers.Sdca(labelColumnName: "Label", featureColumnName: "Features"));

            return trainingPipeline;
        }




        /// <summary>
        /// This method trains the model
        /// </summary>
        /// <param name="pipeline"></param>
        /// <param name="trainSet"></param>
        /// <returns>Returns the trained model</returns>
        private ITransformer TrainModel(EstimatorChain<RegressionPredictionTransformer<LinearRegressionModelParameters>> pipeline, IDataView trainSet)
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
            var modelMetrics = mLContext.Regression.Evaluate(testSetTransform, labelColumnName: "Label", scoreColumnName: "Score");
            DisplayMetrics(modelMetrics);
        }




        /// <summary>
        /// Display metrics from trained model
        /// </summary>
        /// <param name="metrics"></param>
        private void DisplayMetrics(RegressionMetrics modelMetrics)
        {
            Console.WriteLine($"Loss Function: {modelMetrics.LossFunction:0.##}{Environment.NewLine}" +
            $"Mean Absolute Error: {modelMetrics.MeanAbsoluteError:#.##}{Environment.NewLine}" +
            $"Mean Squared Error: {modelMetrics.MeanSquaredError:#.##}{Environment.NewLine}" +
            $"RSquared: {modelMetrics.RSquared:0.##}{Environment.NewLine}" +
            $"Root Mean Squared Error: {modelMetrics.RootMeanSquaredError:#.##}");
        }


        private void PreviewData(MLContext mlContext, IDataView dataView)
        {
            // show data in IDataView
            var preview = dataView.Preview();

            // Print to console
            Console.WriteLine(string.Join("\t", preview.Schema.Select(c => c.Name)));

            // print each row
            foreach (var row in preview.RowView)
            {
                var values = row.Values.Select(v => v.Value.ToString());
                Console.WriteLine(string.Join("\t", values));
            }
        }

    }
}
