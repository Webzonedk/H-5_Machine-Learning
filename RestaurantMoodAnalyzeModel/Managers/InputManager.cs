using RestaurantMoodAnalyzeModel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantMoodAnalyzeModel.Managers
{
    internal class InputManager
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


    }
}
