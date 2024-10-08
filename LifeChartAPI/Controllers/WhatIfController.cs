using LifeChartAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace LifeChartAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WhatIfController : ControllerBase
	{
		private readonly IConfiguration _config;
		private readonly AccountController _accountController;
		public WhatIfController(IConfiguration config, AccountController accountController)
		{
			_config = config;
			_accountController = accountController;
		}

		[HttpGet]
		public IActionResult GetWhatIf()
		{
			string authHeader = Request.Headers.Authorization;
			if (string.IsNullOrEmpty(authHeader))
			{
				return StatusCode(403);
			}
			//Console.WriteLine("Token:" + authHeader);
			string jwt = authHeader.Split(' ')[1];
			string userId = _accountController.ValidateJWT(jwt);
			if (userId == null)
			{
				return StatusCode(403);
			}
			string connectionString = _config.GetConnectionString("LifeChartDatabase");
			WhatIfModel model = new(connectionString, userId);
			return Ok(model);
		}

		[HttpPost("SaveWhatIf")]
		public async Task<IActionResult> SaveWhatIf()
		{
			StreamReader reader = new(Request.Body, Encoding.UTF8);
			var requestContent = await reader.ReadToEndAsync();
			var saveWhatIf = JsonConvert.DeserializeObject<SaveWhatIf>(requestContent);
			string authHeader = Request.Headers.Authorization;
			if (string.IsNullOrEmpty(authHeader))
			{
				return StatusCode(403);
			}
			//Console.WriteLine("Token:" + authHeader);
			string jwt = authHeader.Split(' ')[1];
			string userId = _accountController.ValidateJWT(jwt);
			if (userId == null)
			{
				return StatusCode(403);
			}
			string connectionString = _config.GetConnectionString("LifeChartDatabase");
			//init with no attribute
			WhatIfModel model = new();
			int stages = 0;
			stages += (saveWhatIf.StartStage1 != null && saveWhatIf.StartStage1 != 0) ? 1 : 0;
			stages += (saveWhatIf.StartStage2 != null && saveWhatIf.StartStage2 != 0) ? 1 : 0;
			stages += (saveWhatIf.StartStage3 != null && saveWhatIf.StartStage3 != 0) ? 1 : 0;


			string status = model.SaveWhatIf(connectionString, userId, 
				saveWhatIf.CurrentAge, saveWhatIf.FFPAge, saveWhatIf.MonthlySpending, saveWhatIf.Inflation, 
				saveWhatIf.BankAsset, saveWhatIf.BankROI, stages,
				saveWhatIf.StartStage1, saveWhatIf.EndStage1, saveWhatIf.AnnualIncreaseStage1, saveWhatIf.SaveMonthlyStage1,
				saveWhatIf.StartStage2, saveWhatIf.EndStage2, saveWhatIf.AnnualIncreaseStage2, saveWhatIf.SaveMonthlyStage2,
				saveWhatIf.StartStage3, saveWhatIf.EndStage3, saveWhatIf.AnnualIncreaseStage3, saveWhatIf.SaveMonthlyStage3,
				saveWhatIf.SaveFirstMonth, saveWhatIf.SaveAtStage, saveWhatIf.SaveEnough, saveWhatIf.FFPAtAge);
			if (status == "200")
			{
				return Ok(200);
			}
			return StatusCode(500);
		}

		[HttpPost("SaveSurvey")]
		public async Task<IActionResult> SaveSurvey()
		{
			StreamReader reader = new(Request.Body, Encoding.UTF8);
			var requestContent = await reader.ReadToEndAsync();
			var saveSurvey = JsonConvert.DeserializeObject<SurveyModel>(requestContent);
			string authHeader = Request.Headers.Authorization;
			if (string.IsNullOrEmpty(authHeader))
			{
				return StatusCode(403);
			}
			//Console.WriteLine("Token:" + authHeader);
			string jwt = authHeader.Split(' ')[1];
			string userId = _accountController.ValidateJWT(jwt);
			if (userId == null)
			{
				return StatusCode(403);
			}
			string connectionString = _config.GetConnectionString("LifeChartDatabase");
			//init with no attribute
			SurveyModel model = new();

			string status = model.SaveSurvey(connectionString, userId, saveSurvey.Name, saveSurvey.Age, saveSurvey.Gender);
			if (status == "200")
			{
				return Ok(200);
			}
			return StatusCode(500);
		}
	}

	public class SaveWhatIf {
		public int CurrentAge { get; set; }
		public int FFPAge { get; set; }
		public double MonthlySpending { get; set; }
		public double Inflation { get; set; }
		public double BankAsset { get; set; }
		public double BankROI { get; set; }
		public int StartStage1 { get; set; }
		public int StartStage2 { get; set; }
		public int StartStage3 { get; set; }
		public int EndStage1 { get; set; }
		public int EndStage2 { get; set; }
		public int EndStage3 { get; set; }
		public double AnnualIncreaseStage1 { get; set; }
		public double AnnualIncreaseStage2 { get; set; }
		public double AnnualIncreaseStage3 { get; set; }
		public double SaveMonthlyStage1 { get; set; }
		public double SaveMonthlyStage2 { get; set; }
		public double SaveMonthlyStage3 { get; set; }
		public double SaveFirstMonth { get; set; }
		public int SaveAtStage { get; set; }
		public bool SaveEnough { get; set ; }
		public int FFPAtAge { get; set; }
	}
}
