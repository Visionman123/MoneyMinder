using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using LifeChartAPI.Models;

namespace LifeChartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenGenerationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public TokenGenerationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult generateJWT([FromBody] JwtRequestModel model)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
              _configuration["Jwt:Issuer"],
              model.UID,
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials
            );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(jwt);
        }
    }
}
