using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using LifeChart.Models;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using Azure;

namespace LifeChart.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IConfiguration _configuration;

		public HomeController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration; 
        }


		public async Task<IActionResult> WhatIf()
		{
			Response.Cookies.Append("AnsweredSurvey", "false", new CookieOptions
			{
				Expires = DateTimeOffset.UtcNow.AddDays(1)
			});

			if (string.IsNullOrEmpty(HttpContext.Request.Headers.Authorization))
			{
				Console.WriteLine("Redirecting to logout...");
				return RedirectToAction("Logout", "Account");
			}
			var response = await GetWhatIf();
			if (ModelState.IsValid && response != null)
			{
				WhatIfModel model = response;
				if (model.FFPStages != null && model.FFPStages.Count > 0)
				{
					Console.WriteLine("Yessss");
					Response.Cookies.Append("AnsweredSurvey", "true", new CookieOptions
					{
						Expires = DateTimeOffset.UtcNow.AddDays(1)
					});
				}
				return View(model);
			}
			return RedirectToAction("Logout", "Account");
		}


		public async Task<WhatIfModel> GetWhatIf()
		{
			var client = new HttpClient();
			string apiUrl = _configuration.GetConnectionString("BaseURL") + "/api/WhatIf";

			var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
			//set up auth header
			string authHeader = HttpContext.Request.Headers.Authorization;
			//add auth header
			request.Headers.Add("Authorization", authHeader);

			// Send the GET request with the request body
			HttpResponseMessage response = await client.SendAsync(request);
			//Console.WriteLine(response);
			if (response.IsSuccessStatusCode)
			{
				// Request was successful
				WhatIfModel responseContent = await response.Content.ReadFromJsonAsync<WhatIfModel>();
				// Handle the response content
				return responseContent;
			}
			// Request failed, handle the error
			return null;
		}

		public async Task<IActionResult> SaveWhatIf(
			int currentAge, int ffpAge, double monthlySpending, double inflation, double bankAsset, double bankROI,
			int startStage1, int endStage1, double annualIncreaseStage1, double saveMonthlyStage1,
			int startStage2, int endStage2, double annualIncreaseStage2, double saveMonthlyStage2,
			int startStage3, int endStage3, double annualIncreaseStage3, double saveMonthlyStage3,
			double saveFirstMonth, int saveAtStage)
		{
			try
			{
				var client = new HttpClient();
				string apiUrl = _configuration.GetConnectionString("BaseURL") + "/api/WhatIf/SaveWhatIf";

				var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
				//set up auth header
				string authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
				//set up request body
				var requestBody = new
				{
					CurrentAge = currentAge,
					FFPAge = ffpAge,
					MonthlySpending = monthlySpending,
					Inflation = inflation,
					BankAsset = bankAsset,
					BankROI = bankROI,
					StartStage1 = startStage1,
					StartStage2 = startStage2,
					StartStage3 = startStage3,
					EndStage1 = endStage1,
					EndStage2 = endStage2,
					EndStage3 = endStage3,
					AnnualIncreaseStage1 = annualIncreaseStage1,
					AnnualIncreaseStage2 = annualIncreaseStage2,
					AnnualIncreaseStage3 = annualIncreaseStage3,
					SaveMonthlyStage1 = saveMonthlyStage1,
					SaveMonthlyStage2 = saveMonthlyStage2,
					SaveMonthlyStage3 = saveMonthlyStage3,
					SaveFirstMonth = saveFirstMonth,
					SaveAtStage = saveAtStage
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
					return Ok(response);
				}

				return RedirectToAction("WhatIf");
			}
			catch (Exception ex) 
			{
				Console.WriteLine("Exception occured: " + ex);
				return RedirectToAction("Logout", "Account");
			}

		}

		public async Task<IActionResult> SaveSurvey([FromForm] SurveyModel model)
		{
			try
			{
				var client = new HttpClient();
				string apiUrl = _configuration.GetConnectionString("BaseURL") + "/api/WhatIf/SaveSurvey";

				var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
				//set up auth header
				string authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
				//set up request body
				var requestBody = new
				{
					Name = model.Name,
					Age = model.Age,
					Gender = model.Gender
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
					return Ok(response);
				}

				return RedirectToAction("WhatIf");
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception occured: " + ex);
				return RedirectToAction("Logout", "Account");
			}

		}

		//      public async Task<IActionResult> Dashboard()
		//      {
		//          if (string.IsNullOrEmpty(HttpContext.Request.Headers.Authorization))
		//          {
		//              Console.WriteLine("Redirecting to logout...");
		//              return RedirectToAction("Logout", "Account");
		//          }

		//          var response = await GetDashboard();
		//          if (ModelState.IsValid && response != null)
		//          {
		//              DashboardModel dashboardModel = response;
		//              return View(dashboardModel);
		//          }
		//	return RedirectToAction("Logout", "Account");
		//}

		//      public async Task<DashboardModel> GetDashboard()
		//      {
		//          var client = new HttpClient();
		//          string apiUrl = "https://localhost:7147/api/Dashboard";

		//          var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
		//          //set up auth header
		//          string authHeader = HttpContext.Request.Headers.Authorization;
		//          //add auth header
		//          request.Headers.Add("Authorization", authHeader);

		//          // Send the GET request with the request body
		//          HttpResponseMessage response = await client.SendAsync(request);
		//	//Console.WriteLine(response);
		//	if (response.IsSuccessStatusCode)
		//          {
		//              // Request was successful
		//              DashboardModel responseContent = await response.Content.ReadFromJsonAsync<DashboardModel>();
		//              // Handle the response content
		//              return responseContent;
		//          }
		//	// Request failed, handle the error
		//	return null;
		//}

		//      public async Task<IActionResult> Portfolio()
		//      {
		//          if (string.IsNullOrEmpty(HttpContext.Request.Headers.Authorization))
		//          {
		//              Console.WriteLine("Redirecting to logout...");
		//              return RedirectToAction("Logout", "Account");
		//          }

		//	var response = await GetPortfolio();
		//	if (ModelState.IsValid && response != null)
		//          {
		//              PortfolioModel portfolioModel = response;
		//              return View(portfolioModel);
		//          }
		//	return RedirectToAction("Logout", "Account");
		//}

		//      public async Task<PortfolioModel> GetPortfolio()
		//      {
		//          var client = new HttpClient();
		//          string apiUrl = "https://localhost:7147/api/Portfolio";

		//          var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
		//          //set up auth header
		//          string authHeader = HttpContext.Request.Headers.Authorization;
		//          //add auth header
		//          request.Headers.Add("Authorization", authHeader);

		//          // Send the GET request with the request body
		//          HttpResponseMessage response = await client.SendAsync(request);
		//	if (response.IsSuccessStatusCode)
		//          {
		//              // Request was successful
		//              PortfolioModel responseContent = await response.Content.ReadFromJsonAsync<PortfolioModel>();
		//              // Handle the response content
		//              return responseContent;
		//          }
		//	// Request failed, handle the error
		//	return null;
		//}
		//      public async Task<IActionResult> EditPortfolio(PortfolioModel model, List<string> InvestmentId, List<decimal> InvestmentAmount, List<decimal> RoiAmount, List<string> EstateId, List<decimal> EstateAmount)
		//      {   

		//          List<Investment> investments = new List<Investment>();
		//          for (int i = 0; i < InvestmentAmount.Count; i++) {
		//              int investmentId = Int32.Parse(InvestmentId[i]);
		//              decimal investmentAmount = InvestmentAmount[i];
		//              decimal roiAmount = RoiAmount[i];

		//              //Console.WriteLine(investmentId);
		//              //Console.WriteLine(investmentAmount);
		//              //Console.WriteLine(roiAmount);

		//              Investment investment = new Investment(investmentId, investmentAmount, roiAmount);
		//              //Console.WriteLine(investment);
		//              investments.Add(investment);
		//	}

		//          List<RealEstate> realEstates = new List<RealEstate>(); 
		//	for (int i = 0; i < EstateAmount.Count; i++)
		//	{
		//              int estateId = Int32.Parse(EstateId[i]);
		//              decimal estateAmount = EstateAmount[i];

		//              RealEstate realEstate = new RealEstate(estateId, estateAmount);
		//              //Console.WriteLine(realEstate.Amount);
		//              realEstates.Add(realEstate);
		//	}

		//          //add to model
		//          model.Income.Investments = new();
		//          model.Income.Investments.AddRange(investments);
		//          model.Assets.RealEstates = new();
		//          model.Assets.RealEstates.AddRange(realEstates);

		//	var client = new HttpClient();
		//          string apiUrl = "https://localhost:7147/api/Portfolio/EditPortfolio";

		//          var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
		//          //set up auth header
		//          string authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
		//          //set up request body
		//          var requestBody = new
		//          {
		//              Income = model.Income,
		//              Assets = model.Assets,
		//              Debt = model.Debt,
		//              ExpenseLimit = model.ExpenseLimit,
		//              Date = DateTime.Now,
		//          };
		//          // Serialize the request body to JSON
		//          string jsonRequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

		//          //add auth header
		//          request.Headers.Add("Authorization", authHeader);
		//          //add body
		//          request.Content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");


		//          // Send request with request body
		//          HttpResponseMessage response = await client.SendAsync(request);

		//	if (response.IsSuccessStatusCode)
		//          {
		//              Console.WriteLine("Ok");
		//              // Request was successful
		//              return RedirectToAction("Portfolio");

		//          }
		//          // Request failed, handle the error
		//          return RedirectToAction("Logout", "Account"); ;

		//      }

		//      public async Task<IActionResult> DeleteInvestment(string InvestmentId) 
		//      {
		//          var client = new HttpClient();
		//          string apiUrl = "https://localhost:7147/api/Portfolio/DeleteInvestment";

		//          var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
		//          //set up auth header
		//          string authHeader = HttpContext.Request.Headers.Authorization;
		//          //set up request body
		//          var requestBody = new
		//          {
		//             Id = InvestmentId,
		//          };
		//          // Serialize the request body to JSON
		//          string jsonRequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

		//          //add auth header
		//          request.Headers.Add("Authorization", authHeader);
		//          //add body
		//          request.Content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");


		//          // Send request with request body
		//          HttpResponseMessage response = await client.SendAsync(request);

		//	if (response.IsSuccessStatusCode)
		//          {
		//              // Request was successful
		//              return RedirectToAction("Portfolio");

		//          }   
		//          // Request failed, handle the error
		//          return RedirectToAction("Logout", "Account"); ;

		//      }

		//      public async Task<IActionResult> DeleteRealEstate(string EstateId)
		//      {
		//          var client = new HttpClient();
		//          string apiUrl = "https://localhost:7147/api/Portfolio/DeleteRealEstate";

		//          var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
		//          //set up auth header
		//          string authHeader = HttpContext.Request.Headers.Authorization;
		//          //set up request body
		//          var requestBody = new
		//          {
		//              Id = EstateId,
		//          };
		//          // Serialize the request body to JSON
		//          string jsonRequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

		//          //add auth header
		//          request.Headers.Add("Authorization", authHeader);
		//          //add body
		//          request.Content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");


		//          // Send request with request body
		//          HttpResponseMessage response = await client.SendAsync(request);

		//	if (response.IsSuccessStatusCode)
		//          {
		//              // Request was successful
		//              return RedirectToAction("Portfolio");

		//          }
		//	// Request failed, handle the error
		//	return RedirectToAction("Logout", "Account"); ;
		//}


		//public async Task<IActionResult> MoneyLover()
		//      {
		//          if (string.IsNullOrEmpty(HttpContext.Request.Headers.Authorization))
		//          {
		//              Console.WriteLine("Redirecting to logout...");
		//              return RedirectToAction("Logout", "Account");
		//          }

		//          var response = await GetMoneyLover();
		//          if (ModelState.IsValid && response != null)
		//          {
		//              MoneyLoverModel expenseModel = response;
		//              return View(expenseModel);
		//          }
		//	return RedirectToAction("Logout", "Account");
		//}

		//      public async Task<MoneyLoverModel> GetMoneyLover()
		//      {
		//          var client = new HttpClient();
		//          string apiUrl = "https://localhost:7147/api/MoneyLover";

		//          var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
		//          //set up auth header
		//          string authHeader = HttpContext.Request.Headers.Authorization;
		//          //set up request body
		//          var requestBody = new
		//          { 
		//              Date = "2023-11-08",
		//          };
		//          // Serialize the request body to JSON
		//          string jsonRequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

		//          //add auth header
		//          request.Headers.Add("Authorization", authHeader);
		//          //add body
		//          request.Content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

		//          // Send the GET request with the request body
		//          HttpResponseMessage response = await client.SendAsync(request);

		//	if (response.IsSuccessStatusCode)
		//          {
		//              // Request was successful
		//              MoneyLoverModel responseContent = await response.Content.ReadFromJsonAsync<MoneyLoverModel>();
		//              // Handle the response content
		//              return responseContent;
		//          }
		//	// Request failed, handle the error
		//	return null;
		//}

		//      public async Task<IActionResult> _PastExpenses(string date)
		//      {
		//          if (string.IsNullOrEmpty(HttpContext.Request.Headers.Authorization))
		//          {
		//              return Redirect("../Account/Login");
		//          }

		//          var response = await GetPastExpenses(date);
		//          if (ModelState.IsValid && response != null)
		//          {
		//              MoneyLoverModel pastExpenseModel = response;
		//              return PartialView("_PastExpenses", pastExpenseModel);
		//          }
		//	return RedirectToAction("Logout", "Account");
		//}

		//      public async Task<MoneyLoverModel> GetPastExpenses(string? date)
		//      {
		//          var client = new HttpClient();
		//          string apiUrl = "https://localhost:7147/api/MoneyLover/PastExpenses";

		//          var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
		//          //set up auth header
		//          string authHeader = HttpContext.Request.Headers.Authorization;
		//          //set up request body
		//          var requestBody = new
		//          {
		//              Date = date,
		//          };
		//          // Serialize the request body to JSON
		//          string jsonRequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

		//          //add auth header
		//          request.Headers.Add("Authorization", authHeader);
		//          //add body
		//          request.Content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

		//          // Send the GET request with the request body
		//          HttpResponseMessage response = await client.SendAsync(request);

		//	if (response.IsSuccessStatusCode)
		//          {
		//              // Request was successful
		//              MoneyLoverModel responseContent = await response.Content.ReadFromJsonAsync<MoneyLoverModel>();
		//              // Handle the response content
		//              return responseContent;
		//          }
		//	// Request failed, handle the error
		//	return null;
		//}


		//      public async Task<MoneyLoverModel> GetTodayExpenses()
		//      {
		//          var client = new HttpClient();
		//          string apiUrl = "https://localhost:7147/api/MoneyLover/TodayExpenses";

		//          var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
		//          //set up auth header
		//          string authHeader = HttpContext.Request.Headers.Authorization;
		//          //add auth header
		//          request.Headers.Add("Authorization", authHeader);

		//          // Send the GET request with the request body
		//          HttpResponseMessage response = await client.SendAsync(request);

		//	if (response.IsSuccessStatusCode)
		//          {
		//              // Request was successful
		//              MoneyLoverModel responseContent = await response.Content.ReadFromJsonAsync<MoneyLoverModel>();
		//              Console.WriteLine(responseContent.TodayExpenses);
		//              // Handle the response content
		//              return responseContent;
		//          }
		//          return null;
		//      }


		//      public async Task<IActionResult> AddExpense(int categoryId, decimal amount)
		//      {
		//          var client = new HttpClient();
		//          string apiUrl = "https://localhost:7147/api/MoneyLover/AddExpense";

		//          var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
		//          //set up auth header
		//          string authHeader = HttpContext.Request.Headers.Authorization;
		//          //set up request body
		//          var requestBody = new
		//          {
		//              CategoryId = categoryId,
		//              Amount = amount,
		//              Date = DateTime.Now,
		//          };
		//          // Serialize the request body to JSON
		//          string jsonRequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

		//          //add auth header
		//          request.Headers.Add("Authorization", authHeader);
		//          //add body
		//          request.Content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");


		//          // Send request with request body
		//          HttpResponseMessage response = await client.SendAsync(request);

		//	if (response.IsSuccessStatusCode)
		//          {
		//              MoneyLoverModel responseContent = await response.Content.ReadFromJsonAsync<MoneyLoverModel>();
		//              // Request was successful
		//              return PartialView("_TodayExpenses", responseContent);

		//          }
		//	// Request failed, handle the error
		//	return RedirectToAction("Logout", "Account");
		//}
	}
}

