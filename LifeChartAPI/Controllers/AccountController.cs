using Azure;
using LifeChartAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static OpenIddict.Abstractions.OpenIddictConstants;
using static Plotly.NET.StyleParam.DrawingStyle;

namespace LifeChartAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		public readonly UserManager<IdentityUser> _userManager;
		public readonly SignInManager<IdentityUser> _signInManager;
		public AccountController(IConfiguration configuration, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
		{
			_configuration = configuration;
			_userManager = userManager;
			_signInManager = signInManager;
		}
		//login
		[HttpPost("ProcLogin")]
		public async Task<IActionResult> ProcLogin([FromBody] LoginModel model)
		{
			try
			{
				var user = await AuthenticateUser(model);

				if (user != null)
				{
					var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
					var tokenDescriptor = new SecurityTokenDescriptor
					{
						Subject = new ClaimsIdentity(new Claim[]
						{
							new Claim(ClaimTypes.NameIdentifier, user.Id)
							// Add other claims as needed
						}),
						Expires = DateTime.UtcNow.AddMinutes(120),
						Issuer = _configuration["Jwt:Issuer"],
						Audience = _configuration["Jwt:Audience"],
						SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
					};
					var token = new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);
					//Console.WriteLine(token.ToString());
					var jwt = new JwtSecurityTokenHandler().WriteToken(token);
					//Console.WriteLine(jwt);
					return Ok(jwt);
				}
				return BadRequest("Failed to login");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		private async Task<IdentityUser> AuthenticateUser(LoginModel model)
		{
			IdentityUser user = null;

			string username = model.Username!;
			string password = model.Password!;

			var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);
			if (result.Succeeded)
			{
				//get user in db
				user = await _userManager.FindByNameAsync(username);
			}
			else
			{
				user = await Register(model);
			}
			return user;
		}

		private async Task<IdentityUser> Register(LoginModel model)
		{
			IdentityUser user = null;

			string username = model.Username!;
			string password = model.Password!;

			var newUser = new IdentityUser
			{
				UserName = username,
			};
			//Console.WriteLine(newUser);
			var result = await _userManager.CreateAsync(newUser, password);
			var errors = result.Errors;
			var message = string.Join(", ", errors.Select(x => "Code " + x.Code + " Description" + x.Description));
			//Console.WriteLine(message);
			//Console.WriteLine(result.Succeeded);
			if (result.Succeeded)
			{
				//Console.WriteLine("Success!");
				user = await _userManager.FindByNameAsync(username);
			}
			return user;
		}

		[HttpGet("Authorize")]
		public string ValidateJWT(string jwt)
		{
			//Console.WriteLine("Jwt in AccountController:" + jwt);
			var tokenHandler = new JwtSecurityTokenHandler();
			SecurityToken token = null;

			try
			{
				ClaimsPrincipal principal = tokenHandler.ValidateToken(jwt, new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = false,
					ValidateIssuerSigningKey = true,
					ValidateLifetime = true,
					ClockSkew = TimeSpan.Zero,
					ValidIssuer = _configuration["Jwt:Issuer"],
					ValidAudience = _configuration["Jwt:Audience"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
				}, out token);
				var claimsIdentity = principal.Identity as ClaimsIdentity;
				string userId = null;
				foreach (var claim in claimsIdentity.Claims)
				{
					//Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
					if (claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
					{
						userId = claim.Value;
					}
				}
				return userId;
			}
			catch (Exception ex)
			{
				return null;
			}
		}
	}
}
