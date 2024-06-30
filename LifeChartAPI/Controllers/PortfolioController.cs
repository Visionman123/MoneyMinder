using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LifeChartAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Newtonsoft.Json;

namespace LifeChartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly AccountController _accountController;
        public PortfolioController(IConfiguration config, AccountController accountController)
        {
            _config = config;
            _accountController = accountController;
        }


        //delete investment
        [HttpPost("DeleteInvestment")]
        public async Task<IActionResult> DeleteInvestment()
        {
            StreamReader reader = new(Request.Body, Encoding.UTF8);
            var requestContent = await reader.ReadToEndAsync();
            var investmentDeletion = JsonConvert.DeserializeObject<InvestmentAndEstateDeletion>(requestContent);
            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader))
            {
                return StatusCode(403);
            }
            string jwt = authHeader.Split(' ')[1];
            string userId = _accountController.ValidateJWT(jwt);
            if (userId == null)
            {
                return StatusCode(403);
            }

            string connectionString = _config.GetConnectionString("LifeChartDatabase");
            //init with no attribute
            PortfolioModel model = new();
            string status = model.DeleteInvestment(connectionString, userId, investmentDeletion.Id);
            if (status == "200")
            {
                return Ok(200);
            }
            return StatusCode(500);
        }

        //delete real estate
        [HttpPost("DeleteRealEstate")]
        public async Task<IActionResult> DeleteRealEstate()
        {
            StreamReader reader = new(Request.Body, Encoding.UTF8);
            var requestContent = await reader.ReadToEndAsync();
            var estateDeletion = JsonConvert.DeserializeObject<InvestmentAndEstateDeletion>(requestContent);
            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader))
            {
                return StatusCode(403);
            }
            string jwt = authHeader.Split(' ')[1];
            string userId = _accountController.ValidateJWT(jwt);
            if (userId == null)
            {
                return StatusCode(403);
            }

            string connectionString = _config.GetConnectionString("LifeChartDatabase");
            //init with no attribute
            PortfolioModel model = new();
            string status = model.DeleteRealEstate(connectionString, userId, estateDeletion.Id);
            if (status == "200")
            {
                return Ok(200);
            }
            return StatusCode(500);
        }

        //edit portfolio
        [HttpPost("EditPortfolio")]
        public async Task<IActionResult> EditPortfolio()
        {
            StreamReader reader = new(Request.Body, Encoding.UTF8);
            var requestContent = await reader.ReadToEndAsync();
            var jsonPortfolio = JsonConvert.DeserializeObject<PortfolioModel>(requestContent);
            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader))
            {
                return StatusCode(403);
            }
            string jwt = authHeader.Split(' ')[1];
            string userId = _accountController.ValidateJWT(jwt);
            if (userId == null)
            {
                return StatusCode(403);
            }

            string connectionString = _config.GetConnectionString("LifeChartDatabase");
            //init with no attribute
            PortfolioModel model = new();
            string status = model.EditPortfolio(connectionString, userId, jsonPortfolio.Income, jsonPortfolio.Assets, jsonPortfolio.Debt, jsonPortfolio.ExpenseLimit, jsonPortfolio.Date);
            if (status == "200")
            {
                return Ok(200);
            }
            return StatusCode(500);
        }

        //get portfolio
        [HttpGet]
        public IActionResult GetPortfolio()
        {
            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader))
            {
                return StatusCode(403);
            }
            string jwt = authHeader.Split(' ')[1];
            string userId = _accountController.ValidateJWT(jwt);
            if (userId == null)
            {
                Console.WriteLine("UserId is null");
                return StatusCode(403);
            }

            string connectionString = _config.GetConnectionString("LifeChartDatabase");
            PortfolioModel model = new(connectionString, userId);
            return Ok(model);
        }
    }


    public class InvestmentAndEstateDeletion
    {
        public int? Id { get; set; }
    }
}
