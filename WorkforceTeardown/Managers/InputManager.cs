using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkforceTeardown.Interfaces;
using WorkforceTeardown.Interfaces;

namespace WorkforceTeardown.Managers
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
            Console.WriteLine("\nWelcome to the Workforce Tear down Analyze Model");
            Console.WriteLine("Please choose an option:");
            Console.WriteLine("1. Train the model");
            Console.WriteLine("2. Predict Employee Expected workForce availability");
            Console.WriteLine("3. Exit");

            string? input = Console.ReadLine();
            if (input == "1")
            {
                _trainer.Train();
            }
            else if (input == "2")
            {
                _predictor.Predict();
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
