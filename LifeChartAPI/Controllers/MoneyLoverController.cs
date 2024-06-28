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
        private readonly AccountController _accountController;
        private readonly SpendingBehaviorController _spendingBehaviorController;
        public MoneyLoverController(IConfiguration config, AccountController accountController, SpendingBehaviorController spendingBehaviorController)
        {
            _config = config;
            _accountController = accountController;
            _spendingBehaviorController = spendingBehaviorController;
        }

        //get money lover
        [HttpGet]
        public async Task<IActionResult> GetMoneyLover()
        {
            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader))
            {
                return StatusCode(403);
            }
            string jwt = authHeader.Split(' ')[1];
            string userId = _accountController.ValidateJWT(jwt);
            if (userId == "forbidden")
            {
                return StatusCode(403);
            }

            StreamReader reader = new(Request.Body, Encoding.UTF8);
            var requestContent = await reader.ReadToEndAsync();
            var jsonDate = JsonConvert.DeserializeObject<DateClass>(requestContent);
            string connectionString = _config.GetConnectionString("LifeChartDatabase");

            //prediction 
            //SpendingBehaviorModel groceriesModel = new(connectionString, userId, 1);
            //var groceriesInput = new SpendingModel.ModelInput
            //{
            //    ThreeMonthsPrior = (float)groceriesModel.ThreeMonthsPrior,
            //    TwoMonthsPrior = (float)groceriesModel.TwoMonthsPrior,
            //    OneMonthPrior = (float)groceriesModel.OneMonthPrior,
            //    LimitThisMonth = (float)groceriesModel.Limit
            //};
            //var groceriesOutput = SpendingModel.Predict(groceriesInput);

            //SpendingBehaviorModel entertainmentModel = new(connectionString, userId, 2);
            //var entertainmentInput = new SpendingModel.ModelInput
            //{
            //    ThreeMonthsPrior = (float)entertainmentModel.ThreeMonthsPrior,
            //    TwoMonthsPrior = (float)entertainmentModel.TwoMonthsPrior,
            //    OneMonthPrior = (float)entertainmentModel.OneMonthPrior,
            //    LimitThisMonth = (float)entertainmentModel.Limit
            //};
            //var entertainmentOutput = SpendingModel.Predict(entertainmentInput);

            //SpendingBehaviorModel utilitiesModel = new(connectionString, userId, 3);
            //var utilitiesInput = new SpendingModel.ModelInput
            //{
            //    ThreeMonthsPrior = (float)utilitiesModel.ThreeMonthsPrior,
            //    TwoMonthsPrior = (float)utilitiesModel.TwoMonthsPrior,
            //    OneMonthPrior = (float)utilitiesModel.OneMonthPrior,
            //    LimitThisMonth = (float)utilitiesModel.Limit
            //};
            //var utilitiesOutput = SpendingModel.Predict(utilitiesInput);

            //SpendingBehaviorModel othersModel = new(connectionString, userId, 6);
            //var othersInput = new SpendingModel.ModelInput
            //{
            //    ThreeMonthsPrior = (float)othersModel.ThreeMonthsPrior,
            //    TwoMonthsPrior = (float)othersModel.TwoMonthsPrior,
            //    OneMonthPrior = (float)othersModel.OneMonthPrior,
            //    LimitThisMonth = (float)othersModel.Limit
            //};
            //var othersOutput = SpendingModel.Predict(othersInput);
            PredictionOutput prediction = _spendingBehaviorController.PredictSpending(userId);

            //if date is not supplied then choose a date long ago => No data retrieved
            MoneyLoverModel model = new();
            model.GetPastExpenses(connectionString, userId, (jsonDate.Date != null) ? DateTime.Parse(jsonDate.Date) : DateTime.Parse("1900-01-01"));
            model.GetTodayExpenses(connectionString, userId);
            model.GetExpenseTracker(connectionString, userId);
            model.GroceriesEstimation = (decimal)prediction.GroceriesScore;
            model.EntertainmentEstimation = (decimal)prediction.EntertainmentScore;
            model.UtilitiesEstimation = (decimal)prediction.UtilitiesScore;
            model.OthersEstimation = (decimal)prediction.OthersScore;
            return Ok(model);
        }


        [HttpGet("PastExpenses")]
        public async Task<IActionResult> GetPastExpenses()
        {
            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader))
            {
                return StatusCode(403);
            }
            string jwt = authHeader.Split(' ')[1];
            string userId = _accountController.ValidateJWT(jwt);
            if (userId == "forbidden")
            {
                return StatusCode(403);
            }

            StreamReader reader = new(Request.Body, Encoding.UTF8);
            var requestContent = await reader.ReadToEndAsync();
            var jsonDate = JsonConvert.DeserializeObject<DateClass>(requestContent);
            string connectionString = _config.GetConnectionString("LifeChartDatabase");

            //if date is not supplied then choose a date long ago => No data retrieved
            MoneyLoverModel model = new();
            model.GetPastExpenses(connectionString, userId, (jsonDate.Date != null) ? DateTime.Parse(jsonDate.Date) : DateTime.Parse("1900-01-01"));
            return Ok(model);
        }

        [HttpGet("TodayExpenses")]
        public IActionResult GetTodayExpenses()
        {
            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader))
            {
                return StatusCode(403);
            }
            string jwt = authHeader.Split(' ')[1];
            string userId = _accountController.ValidateJWT(jwt);
            if (userId == "forbidden")
            {
                return StatusCode(403);
            }

            string connectionString = _config.GetConnectionString("LifeChartDatabase");
            MoneyLoverModel model = new();
            model.GetTodayExpenses(connectionString, userId);
            return Ok(model);
        }

        [HttpPost("AddExpense")]
        public async Task<IActionResult> AddExpense()
        {
            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader))
            {
                return StatusCode(403);
            }
            string jwt = authHeader.Split(' ')[1];
            string userId = _accountController.ValidateJWT(jwt);
            if (userId == "forbidden")
            {
                return StatusCode(403);
            }

            StreamReader reader = new(Request.Body, Encoding.UTF8);
            var requestContent = await reader.ReadToEndAsync();
            var jsonExpense = JsonConvert.DeserializeObject<Expense>(requestContent);
            Console.WriteLine(jsonExpense.CategoryId);
            Console.WriteLine(jsonExpense.Amount);
            Console.WriteLine(jsonExpense.Date);
            string connectionString = _config.GetConnectionString("LifeChartDatabase");

            //if date is not supplied then choose a date long ago => No data retrieved
            MoneyLoverModel model = new();
            string status = model.AddExpense(connectionString, userId, jsonExpense.CategoryId, jsonExpense.Amount, DateTime.Parse(jsonExpense.Date));
            //update model
            model.GetTodayExpenses(connectionString, userId);
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
            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader))
            {
                return StatusCode(403);
            }
            string jwt = authHeader.Split(' ')[1];
            string userId = _accountController.ValidateJWT(jwt);
            if (userId == "forbidden")
            {
                return StatusCode(403);
            }

            string connectionString = _config.GetConnectionString("LifeChartDatabase");
            MoneyLoverModel model = new();
            model.GetExpenseTracker(connectionString, userId);
            return Ok(model);
        }
    }

    public class DateClass
    {
        public string? Date { get; set; }
    }

    public class Expense
    {
        public int CategoryId { get; set; }
        public decimal Amount { get; set; }
        public string Date { get; set; }
    }
}


