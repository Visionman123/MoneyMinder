using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using LifeChart.Models;
using System.Text;

namespace LifeChart.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;


        public HomeController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Dashboard()
        {
            if (string.IsNullOrEmpty(_httpContextAccessor.HttpContext.Request.Headers["Authorization"]))
            {
                Console.WriteLine("Redirecting to logout...");
                return RedirectToAction("Logout", "Account");
            }
            
            if (ModelState.IsValid)
            {
                var dashboardModel = await GetDashboard();
                return View(dashboardModel);
            }

            return View();
        }

        public async Task<DashboardModel> GetDashboard()
        {
            var client = new HttpClient();
            string apiUrl = "https://localhost:7147/api/Dashboard";

            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            //set up auth header
            string authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            //add auth header
            request.Headers.Add("Authorization", authHeader);

            // Send the GET request with the request body
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                // Request was successful
                DashboardModel responseContent = await response.Content.ReadFromJsonAsync<DashboardModel>();
                // Handle the response content
                return responseContent;
            }
            else
            {
                // Request failed, handle the error
                return null;
            }
        }

        [Authorize]
        public async Task<IActionResult> Portfolio()
        {
            if (string.IsNullOrEmpty(_httpContextAccessor.HttpContext.Request.Headers["Authorization"]))
            {
                Console.WriteLine("Redirecting to logout...");
                return RedirectToAction("Logout", "Account");
            }

            if (ModelState.IsValid)
            {
                var portfolioModel = await GetPortfolio();
                return View(portfolioModel);
            }
            return View();
        }

		[Authorize]
        public async Task<PortfolioModel> GetPortfolio()
        {
            var client = new HttpClient();
            string apiUrl = "https://localhost:7147/api/Portfolio";

            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            //set up auth header
            string authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            //add auth header
            request.Headers.Add("Authorization", authHeader);

            // Send the GET request with the request body
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                // Request was successful
                PortfolioModel responseContent = await response.Content.ReadFromJsonAsync<PortfolioModel>();
                // Handle the response content
                return responseContent;
            }
            else
            {
                // Request failed, handle the error
                return null;
            }
        }

        [Authorize]
        public async Task<IActionResult> EditPortfolio(PortfolioModel model, List<string> InvestmentId, List<decimal> InvestmentAmount, List<decimal> RoiAmount, List<string> EstateId, List<decimal> EstateAmount)
        {   
            
            List<Investment> investments = new List<Investment>();
            for (int i = 0; i < InvestmentAmount.Count; i++) {
                int investmentId = Int32.Parse(InvestmentId[i]);
                decimal investmentAmount = InvestmentAmount[i];
                decimal roiAmount = RoiAmount[i];

                //Console.WriteLine(investmentId);
                //Console.WriteLine(investmentAmount);
                //Console.WriteLine(roiAmount);

                Investment investment = new Investment(investmentId, investmentAmount, roiAmount);
                Console.WriteLine(investment);
                investments.Add(investment);
			}
			
            List<RealEstate> realEstates = new List<RealEstate>(); 
			for (int i = 0; i < EstateAmount.Count; i++)
			{
                int estateId = Int32.Parse(EstateId[i]);
                decimal estateAmount = EstateAmount[i];

                RealEstate realEstate = new RealEstate(estateId, estateAmount);
                Console.WriteLine(realEstate.Amount);
                realEstates.Add(realEstate);
			}

            //add to model
            model.Income.Investments = new();
            model.Income.Investments.AddRange(investments);
            model.Assets.RealEstates = new();
            model.Assets.RealEstates.AddRange(realEstates);

			var client = new HttpClient();
            string apiUrl = "https://localhost:7147/api/Portfolio/EditPortfolio";

            var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            //set up auth header
            string authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            //set up request body
            var requestBody = new
            {
                Income = model.Income,
                Assets = model.Assets,
                Debt = model.Debt,
                ExpenseLimit = model.ExpenseLimit,
                Date = DateTime.Now,
            };
            // Serialize the request body to JSON
            string jsonRequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

            //add auth header
            request.Headers.Add("Authorization", authHeader);
            //add body
            request.Content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");


            // Send request with request body
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Ok");
                // Request was successful
                return RedirectToAction("Portfolio");

            }
            else
            {
                // Request failed, handle the error
                return StatusCode(500);
            }
        }

        [Authorize]
        public async Task<IActionResult> DeleteInvestment(string InvestmentId) 
        {
            var client = new HttpClient();
            string apiUrl = "https://localhost:7147/api/Portfolio/DeleteInvestment";

            var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            //set up auth header
            string authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            //set up request body
            var requestBody = new
            {
               Id = InvestmentId,
            };
            // Serialize the request body to JSON
            string jsonRequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

            //add auth header
            request.Headers.Add("Authorization", authHeader);
            //add body
            request.Content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");


            // Send request with request body
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                // Request was successful
                return RedirectToAction("Portfolio");

            }
            else
            {
                // Request failed, handle the error
                return StatusCode(500);
            }
        }

        [Authorize]
        public async Task<IActionResult> DeleteRealEstate(string EstateId)
        {
            var client = new HttpClient();
            string apiUrl = "https://localhost:7147/api/Portfolio/DeleteRealEstate";

            var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            //set up auth header
            string authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            //set up request body
            var requestBody = new
            {
                Id = EstateId,
            };
            // Serialize the request body to JSON
            string jsonRequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

            //add auth header
            request.Headers.Add("Authorization", authHeader);
            //add body
            request.Content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");


            // Send request with request body
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                // Request was successful
                return RedirectToAction("Portfolio");

            }
            else
            {
                // Request failed, handle the error
                return StatusCode(500);
            }
        }

        [Authorize]
        public IActionResult WhatIf()
        {
            if (string.IsNullOrEmpty(_httpContextAccessor.HttpContext.Request.Headers["Authorization"]))
            {
                Console.WriteLine("Redirecting to logout...");
                return RedirectToAction("Logout", "Account");
            }
            return View();
        }

        [Authorize]
        public async Task<IActionResult> MoneyLover()
        {
            if (string.IsNullOrEmpty(_httpContextAccessor.HttpContext.Request.Headers["Authorization"]))
            {
                Console.WriteLine("Redirecting to logout...");
                return RedirectToAction("Logout", "Account");
            }

            if (ModelState.IsValid)
            {
                MoneyLoverModel expenseModel = await GetMoneyLover();
                return View(expenseModel);
            }
            return View();
        }

        [Authorize]
        public async Task<MoneyLoverModel> GetMoneyLover()
        {
            var client = new HttpClient();
            string apiUrl = "https://localhost:7147/api/MoneyLover";

            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            //set up auth header
            string authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            //set up request body
            var requestBody = new
            { 
                Date = "2023-11-08",
            };
            // Serialize the request body to JSON
            string jsonRequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

            //add auth header
            request.Headers.Add("Authorization", authHeader);
            //add body
            request.Content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

            // Send the GET request with the request body
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                // Request was successful
                MoneyLoverModel responseContent = await response.Content.ReadFromJsonAsync<MoneyLoverModel>();
                // Handle the response content
                return responseContent;
            }
            else
            {
                // Request failed, handle the error
                return null;
            }
        }

        public async Task<IActionResult> _PastExpenses(string date)
        {
            if (string.IsNullOrEmpty(_httpContextAccessor.HttpContext.Request.Headers["Authorization"]))
            {
                return Redirect("../Account/Login");
            }

            if (ModelState.IsValid)
            {
                MoneyLoverModel pastExpenseModel = await GetPastExpenses(date);
                return PartialView("_PastExpenses", pastExpenseModel);
            }
            return PartialView("_PastExpenses");
        }

        [Authorize]
        public async Task<MoneyLoverModel> GetPastExpenses(string? date)
        {
            var client = new HttpClient();
            string apiUrl = "https://localhost:7147/api/MoneyLover/PastExpenses";

            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            //set up auth header
            string authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            //set up request body
            var requestBody = new
            {
                Date = date,
            };
            // Serialize the request body to JSON
            string jsonRequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

            //add auth header
            request.Headers.Add("Authorization", authHeader);
            //add body
            request.Content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

            // Send the GET request with the request body
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                // Request was successful
                MoneyLoverModel responseContent = await response.Content.ReadFromJsonAsync<MoneyLoverModel>();
                // Handle the response content
                return responseContent;
            }
            else
            {
                // Request failed, handle the error
                return null;
            }
        }


        [Authorize]
        public async Task<MoneyLoverModel> GetTodayExpenses()
        {
            var client = new HttpClient();
            string apiUrl = "https://localhost:7147/api/MoneyLover/TodayExpenses";

            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            //set up auth header
            string authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            //add auth header
            request.Headers.Add("Authorization", authHeader);

            // Send the GET request with the request body
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                // Request was successful
                MoneyLoverModel responseContent = await response.Content.ReadFromJsonAsync<MoneyLoverModel>();
                Console.WriteLine(responseContent.TodayExpenses);
                // Handle the response content
                return responseContent;
            }
            else
            {
                // Request failed, handle the error
                return null;
            }
        }


        [Authorize]
        public async Task<IActionResult> AddExpense(string category, decimal amount)
        {
            var client = new HttpClient();
            string apiUrl = "https://localhost:7147/api/MoneyLover/AddExpense";

            var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            //set up auth header
            string authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            //set up request body
            var requestBody = new
            {
                Category = category,
                Amount = amount,
                Date = DateTime.Now,
            };
            // Serialize the request body to JSON
            string jsonRequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

            //add auth header
            request.Headers.Add("Authorization", authHeader);
            //add body
            request.Content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");


            // Send request with request body
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                MoneyLoverModel responseContent = await response.Content.ReadFromJsonAsync<MoneyLoverModel>();
                // Request was successful
                return PartialView("_TodayExpenses", responseContent);

            }
            else
            {
                // Request failed, handle the error
                return StatusCode(500);
            }
        }
    }
}
