
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestaurantMoodAnalyzeModel.Interfaces;
using RestaurantMoodAnalyzeModel.ML;
using RestaurantMoodAnalyzeModel.ML.Base;

namespace RestaurantMoodAnalyzeModel
{
    class Program
    {
        public static IConfiguration? Configuration;
        public static ITrainer _trainer;

        public Program(ITrainer trainer)
        {
            _trainer = trainer;
        }
        static void main(string[] args)
        {

            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();


            var serviceProvider = new ServiceCollection()
                .AddSingleton(Configuration)
                .AddTransient<ITrainer, Trainer>()
                .AddTransient<IPredictor, Predictor>()
                .AddSingleton<IBaseML,BaseML>()
                .AddTransient<Program, Program>()
                .BuildServiceProvider();

            var program = ActivatorUtilities.CreateInstance<Program>(serviceProvider);
            program.Run();

        }
        public void Run()
        {
            _trainer.Train();
        }

    }




}