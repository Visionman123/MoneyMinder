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

        [HttpGet]
        public IActionResult PredictSpending()
        {
            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader))
            {
                return StatusCode(500);
            }
            string jwt = authHeader.Split(' ')[1];
            var token = new JwtSecurityTokenHandler().ReadToken(jwt) as JwtSecurityToken;
            var userId = token.Audiences.FirstOrDefault();
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

            PredictionOutput prediction = new(groceriesOutput.Score, entertainmentOutput.Score);
            //return predicted price
            return Ok(prediction);
        }
    }

    public class PredictionOutput
    {
        public float GroceriesScore { get; set; }
        public float EntertainmentScore { get; set; }

        public PredictionOutput(float groceriesScore, float entertainmentScore)
        {
            GroceriesScore = groceriesScore;
            EntertainmentScore = entertainmentScore;
        }
    }
}
