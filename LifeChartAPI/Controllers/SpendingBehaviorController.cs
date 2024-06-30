using LifeChartAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SpendingBehavior;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace LifeChartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpendingBehaviorController : ControllerBase
    {
        private readonly IConfiguration _config;
        public SpendingBehaviorController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        public PredictionOutput PredictSpending(string userId)
        {
            string connectionString = _config.GetConnectionString("LifeChartDatabase");

            SpendingBehaviorModel groceriesModel = new(connectionString, userId, 1);
            var groceriesInput = new SpendingModel.ModelInput
            {
                ThreeMonthsPrior = (float) groceriesModel.ThreeMonthsPrior,
                TwoMonthsPrior = (float) groceriesModel.TwoMonthsPrior,
                OneMonthPrior = (float) groceriesModel.OneMonthPrior,
                LimitThisMonth = (float) groceriesModel.Limit
            };
            var groceriesOutput = SpendingModel.Predict(groceriesInput);

            SpendingBehaviorModel entertainmentModel = new(connectionString, userId, 2);
            var entertainmentInput = new SpendingModel.ModelInput
            {
                ThreeMonthsPrior = (float)entertainmentModel.ThreeMonthsPrior,
                TwoMonthsPrior = (float)entertainmentModel.TwoMonthsPrior,
                OneMonthPrior = (float)entertainmentModel.OneMonthPrior,
                LimitThisMonth = (float)entertainmentModel.Limit
            };
            var entertainmentOutput = SpendingModel.Predict(entertainmentInput);

            SpendingBehaviorModel utilitiesModel = new(connectionString, userId, 3);
            var utilitiesInput = new SpendingModel.ModelInput
            {
                ThreeMonthsPrior = (float)utilitiesModel.ThreeMonthsPrior,
                TwoMonthsPrior = (float)utilitiesModel.TwoMonthsPrior,
                OneMonthPrior = (float)utilitiesModel.OneMonthPrior,
                LimitThisMonth = (float)utilitiesModel.Limit
            };
            var utilitiesOutput = SpendingModel.Predict(utilitiesInput);

            SpendingBehaviorModel othersModel = new(connectionString, userId, 6);
            var othersInput = new SpendingModel.ModelInput
            {
                ThreeMonthsPrior = (float)othersModel.ThreeMonthsPrior,
                TwoMonthsPrior = (float)othersModel.TwoMonthsPrior,
                OneMonthPrior = (float)othersModel.OneMonthPrior,
                LimitThisMonth = (float)othersModel.Limit
            };
            var othersOutput = SpendingModel.Predict(othersInput);

            PredictionOutput prediction = new(groceriesOutput.Score, entertainmentOutput.Score, utilitiesOutput.Score, othersOutput.Score);
            Console.WriteLine(prediction.GroceriesScore);
			Console.WriteLine(prediction.EntertainmentScore);
			Console.WriteLine(prediction.UtilitiesScore);
			Console.WriteLine(prediction.OthersScore);
			//return predicted price
			return prediction;
        }
    }

    public class PredictionOutput
    {
        public float GroceriesScore { get; set; }
        public float EntertainmentScore { get; set; }
        public float UtilitiesScore { get; set; }
        public float OthersScore { get; set; }

        public PredictionOutput(float groceriesScore, float entertainmentScore , float utilitiesScore, float othersScore)
        {
            GroceriesScore = groceriesScore;
            EntertainmentScore = entertainmentScore;
            UtilitiesScore = utilitiesScore;
            OthersScore = othersScore;
        }
    }
}
