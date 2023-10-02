
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestaurantMoodAnalyzeModel.Interfaces;
using RestaurantMoodAnalyzeModel.Managers;
using RestaurantMoodAnalyzeModel.ML;
using RestaurantMoodAnalyzeModel.ML.Base;

namespace RestaurantMoodAnalyzeModel
{
    class Program
    {
        public static IConfiguration? Configuration;
        public static IInputManager _inputManager;

        public Program(IInputManager inputManager)
        {
            _inputManager = inputManager;
        }
        static void Main(string[] args)
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
                .AddTransient<IInputManager, InputManager>()
                .AddTransient<Program, Program>()
                .BuildServiceProvider();

            var program = ActivatorUtilities.CreateInstance<Program>(serviceProvider);
            program.Run();

        }
        public void Run()
        {
            _inputManager.Run();
        }

    }




}