using ForexAnalyzer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ForexAnalyzer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForexForecast : ControllerBase
    {

        private readonly IPredictor _predictor;
        private readonly ITrainer _trainer;

        public ForexForecast(
            IPredictor predictor,
            ITrainer trainer
            )
        {
            _predictor = predictor;
            _trainer = trainer;
        }


        [HttpGet("TrainModel")]
        public IActionResult TrainModel()
        {
            _trainer.Train();
            return Ok();
        }

        [HttpGet("GetEurUsdForecast")]
        public IActionResult Get()
        {
            var forecasts = _predictor.Predict();
            return Ok(forecasts);
        }



    }
}
