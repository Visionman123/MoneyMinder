using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using LifeChartAPI.Models;
using System.Text;
using Newtonsoft.Json;
using SpendingBehavior;

namespace LifeChartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoneyLoverController : ControllerBase
    {
        private readonly IConfiguration _config;
        public MoneyLoverController(IConfiguration config)
        {
            _config = config;
        }
    
        //get money lover
        [HttpGet]
        public async Task<IActionResult> GetMoneyLover()
        {
            //string authHeader = Request.Headers["Authorization"];
            //if (string.IsNullOrEmpty(authHeader))
            //{
            //    return StatusCode(500);
            //}
            //string jwt = authHeader.Split(' ')[1];
            //var token = new JwtSecurityTokenHandler().ReadToken(jwt) as JwtSecurityToken;
            //var userId = token.Audiences.FirstOrDefault();
            StreamReader reader = new(Request.Body, Encoding.UTF8);
            var requestContent = await reader.ReadToEndAsync();
            var jsonDate = JsonConvert.DeserializeObject<DateClass>(requestContent);
            string connectionString = _config.GetConnectionString("LifeChartDatabase");

            //prediction model
            SpendingBehaviorModel groceriesModel = new(connectionString, "balls", "groceries");
            var groceriesInput = new SpendingModel.ModelInput
            {
                ThreeMonthsPrior = (float)groceriesModel.ThreeMonthsPrior,
                TwoMonthsPrior = (float)groceriesModel.TwoMonthsPrior,
                OneMonthPrior = (float)groceriesModel.OneMonthPrior,
                LimitThisMonth = (float)groceriesModel.Limit
            };
            var groceriesOutput = SpendingModel.Predict(groceriesInput);

            SpendingBehaviorModel entertainmentModel = new(connectionString, "balls", "entertainment");
            var entertainmentInput = new SpendingModel.ModelInput
            {
                ThreeMonthsPrior = (float)entertainmentModel.ThreeMonthsPrior,
                TwoMonthsPrior = (float)entertainmentModel.TwoMonthsPrior,
                OneMonthPrior = (float)entertainmentModel.OneMonthPrior,
                LimitThisMonth = (float)entertainmentModel.Limit
            };
            var entertainmentOutput = SpendingModel.Predict(entertainmentInput);

            //if date is not supplied then choose a date long ago => No data retrieved
            MoneyLoverModel model = new();
            model.GetPastExpenses(connectionString, "balls", (jsonDate.Date != null) ? DateTime.Parse(jsonDate.Date) : DateTime.Parse("1900-01-01"));
            model.GetTodayExpenses(connectionString, "balls");
            model.GetExpenseTracker(connectionString, "balls");
            model.GroceriesEstimation = (decimal)groceriesOutput.Score;
            model.EntertainmentEstimation = (decimal)entertainmentOutput.Score;
            return Ok(model);
        }


        [HttpGet("PastExpenses")]
        public async Task<IActionResult> GetPastExpenses()
        {
            //string authHeader = Request.Headers["Authorization"];
            //if (string.IsNullOrEmpty(authHeader))
            //{
            //    return StatusCode(500);
            //}
            //string jwt = authHeader.Split(' ')[1];
            //var token = new JwtSecurityTokenHandler().ReadToken(jwt) as JwtSecurityToken;
            //var userId = token.Audiences.FirstOrDefault();
            StreamReader reader = new(Request.Body, Encoding.UTF8);
            var requestContent = await reader.ReadToEndAsync();
            var jsonDate = JsonConvert.DeserializeObject<DateClass>(requestContent);
            string connectionString = _config.GetConnectionString("LifeChartDatabase");

            //if date is not supplied then choose a date long ago => No data retrieved
            MoneyLoverModel model = new();
            model.GetPastExpenses(connectionString, "balls", (jsonDate.Date != null) ? DateTime.Parse(jsonDate.Date) : DateTime.Parse("1900-01-01"));
            return Ok(model);
        }

        [HttpGet("TodayExpenses")]
        public IActionResult GetTodayExpenses()
        {
            //string authHeader = Request.Headers["Authorization"];
            //if (string.IsNullOrEmpty(authHeader))
            //{
            //    return StatusCode(500);
            //}
            //string jwt = authHeader.Split(' ')[1];
            //var token = new JwtSecurityTokenHandler().ReadToken(jwt) as JwtSecurityToken;
            //var userId = token.Audiences.FirstOrDefault();
            string connectionString = _config.GetConnectionString("LifeChartDatabase");
            MoneyLoverModel model = new();
            model.GetTodayExpenses(connectionString, "balls");
            return Ok(model);
        }

        [HttpPost("AddExpense")]
        public async Task<IActionResult> AddExpense()
        {
            //string authHeader = Request.Headers["Authorization"];
            //if (string.IsNullOrEmpty(authHeader))
            //{
            //    return StatusCode(500);
            //}
            //string jwt = authHeader.Split(' ')[1];
            //var token = new JwtSecurityTokenHandler().ReadToken(jwt) as JwtSecurityToken;
            //var userId = token.Audiences.FirstOrDefault();
            StreamReader reader = new(Request.Body, Encoding.UTF8);
            var requestContent = await reader.ReadToEndAsync();
            var jsonEpxense = JsonConvert.DeserializeObject<Expense>(requestContent);
            string connectionString = _config.GetConnectionString("LifeChartDatabase");

            //if date is not supplied then choose a date long ago => No data retrieved
            MoneyLoverModel model = new();
            string status = model.AddExpense(connectionString, "balls", jsonEpxense.Category, jsonEpxense.Amount, DateTime.Parse(jsonEpxense.Date));
            //update model
            model.GetTodayExpenses(connectionString, "balls");
            if (status == "200")
            {
                return Ok(model);
            }
            else
            {
                return StatusCode(500);
            }
        }

        [HttpGet("ExpenseTracker")]
        public IActionResult GetExpenseTracker()
        {
            //string authHeader = Request.Headers["Authorization"];
            //if (string.IsNullOrEmpty(authHeader))
            //{
            //    return StatusCode(500);
            //}
            //string jwt = authHeader.Split(' ')[1];
            //var token = new JwtSecurityTokenHandler().ReadToken(jwt) as JwtSecurityToken;
            //var userId = token.Audiences.FirstOrDefault();
            string connectionString = _config.GetConnectionString("LifeChartDatabase");
            MoneyLoverModel model = new();
            model.GetExpenseTracker(connectionString, "balls");
            return Ok(model);
        }
    }

    public class DateClass
    {
        public string? Date { get; set; }
    }

    public class Expense
    {
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public string Date { get; set; }
    }
}


