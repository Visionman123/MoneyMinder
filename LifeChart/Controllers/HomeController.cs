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
			double saveFirstMonth, int saveAtStage, bool saveEnough, int ffpAtAge)
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
					SaveAtStage = saveAtStage,
					SaveEnough = saveEnough,
					FFPAtAge = ffpAtAge,
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
				Response.Cookies.Append("AnsweredSurvey", "true", new CookieOptions
				{
					Expires = DateTimeOffset.UtcNow.AddDays(1)
				});


				var client = new HttpClient();
				string apiUrl = _configuration.GetConnectionString("BaseURL") + "/api/WhatIf/SaveSurvey";

				var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
				//set up auth header
				string authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
				//set up request body
				var requestBody = new
				{
					Intake = model.Intake,
					Major = model.Major,
					Gender = model.Gender,
					FFPAchieveAge = model.FFPAchieveAge,
					FFPStrategy = model.FFPStrategy,
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
	}
}

