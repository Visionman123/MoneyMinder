using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LifeChartAPI.Models;
using System.IdentityModel.Tokens.Jwt;

namespace LifeChartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IConfiguration _config;
        public DashboardController(IConfiguration config)
        {
            _config = config;
        }

        //get dashboard
        [HttpGet]
        public IActionResult GetDashBoard()
        {
            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader))
            {
                return StatusCode(403);
            }
            string jwt = authHeader.Split(' ')[1];
            var token = new JwtSecurityTokenHandler().ReadToken(jwt) as JwtSecurityToken;
            var userId = token.Audiences.FirstOrDefault();
            string connectionString = _config.GetConnectionString("LifeChartDatabase");
            DashboardModel model = new(connectionString, userId);
            return Ok(model);
        }
    }
}
