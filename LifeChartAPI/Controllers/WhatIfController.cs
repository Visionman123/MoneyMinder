using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

		//get dashboard
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
			//DashboardModel model = new(connectionString, userId);
			//return Ok(model);
			return Ok();
		}
	}
}
