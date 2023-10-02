using RestaurantMoodAnalyzeModel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantMoodAnalyzeModel.Managers
{
    internal class InputManager : IInputManager
    {

        private readonly IPredictor _predictor;
        private readonly ITrainer _trainer;
        private readonly IBaseML baseML;

        public InputManager(
            IPredictor predictor,
            ITrainer trainer,
            IBaseML baseML)
        {
            _predictor = predictor;
            _trainer = trainer;
            this.baseML = baseML;
        }

        public void Run()
        {
            while (true)
            {
                ShowMenu();
            }
        }


        private void ShowMenu()
        {
            Console.WriteLine("Welcome to the Restaurant Mood Analyze Model");
            Console.WriteLine("Please choose an option:");
            Console.WriteLine("1. Train the model");
            Console.WriteLine("2. Predict");
            Console.WriteLine("3. Exit");

            string? input = Console.ReadLine();
            if (input == "1")
            {
                _trainer.Train();
            }
            else if (input == "2")
            {
                Console.WriteLine("Please enter your feedback:");
                string? feedback = Console.ReadLine();
                if (feedback == null)
                {
                    Console.WriteLine("Invalid input");
                    return;
                }
                _predictor.Predict(feedback);
            }
            else if (input == "3")
            {
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine("Invalid input");
            }
        }


    }
}
