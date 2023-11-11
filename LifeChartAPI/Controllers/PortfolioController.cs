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
        public PortfolioController(IConfiguration config)
        {
            _config = config;
        }


        //edit portfolio
        [HttpPost]
        public async Task<IActionResult> EditPortfolio()
        {
            StreamReader reader = new(Request.Body, Encoding.UTF8);
            var requestContent = await reader.ReadToEndAsync();
            var jsonPortfolio = JsonConvert.DeserializeObject<PortfolioModel>(requestContent);
            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader))
            {
                return StatusCode(500);
            }
            string jwt = authHeader.Split(' ')[1];
            var token = new JwtSecurityTokenHandler().ReadToken(jwt) as JwtSecurityToken;
            var userId = token.Audiences.FirstOrDefault();
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
                return StatusCode(500);
            }
            string jwt = authHeader.Split(' ')[1];
            var token = new JwtSecurityTokenHandler().ReadToken(jwt) as JwtSecurityToken;
            var userId = token.Audiences.FirstOrDefault();
            string connectionString = _config.GetConnectionString("LifeChartDatabase");
            PortfolioModel model = new(connectionString, userId);
            return Ok(model);
        }
    }
}
