using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using LifeChartServices.Services;
using LifeChartServices.Models;
using LifeChart.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace LifeChart.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        public readonly UserManager<IdentityUser> _userManager;
        public readonly SignInManager<IdentityUser> _signInManager;
        public readonly IEmailService _emailService;
        public AccountController(IConfiguration configuration, UserManager<IdentityUser> userManager, IEmailService emailService, SignInManager<IdentityUser> signInManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _emailService = emailService;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        //login
        public async Task<IActionResult> Login(LoginModel model)
        {
			if (ModelState.IsValid)
            {
                var response = await AuthenticateUser(model);

                if (response != null && response != "Bad")
                {
                    var tokenString = response;
                    //Save token in session object
                    if (tokenString != null)
                    {
                        HttpContext.Session.SetString("jwtoken", tokenString);

						// Save the preference in a cookie
						Response.Cookies.Append("LoggedIn", "true", new CookieOptions
						{
							Expires = DateTimeOffset.UtcNow.AddYears(1) 
						});

						// Retrieve the language preference from the cookie
						var languagePreference = Request.Cookies["UserLanguagePreference"] ?? "en-US";


						// Redirect based on language preference
						if (languagePreference == "de-DE")
						{
							return Redirect("../Home/WhatIf?culture=de-DE");
						}

						return Redirect("../Home/WhatIf?culture=en-US"); ;
                    }
                    else
                    {
                        return View();
                    }
                }
                else if (response == "Bad")
                {
					ViewBag.LoginErrorMessage = "Wrong username or password";
				}
                else
                {
					ViewBag.LoginErrorMessage = "Server error";
				}
            }
			return View();
        }

        private async Task<string> AuthenticateUser(LoginModel model)
        {
			var client = new HttpClient();
            // Define the URL of the API endpoint where you want to send the POST request
            string apiUrl = _configuration.GetConnectionString("BaseURL") + "/api/Account/ProcLogin";

			//set up request body
			var requestBody = new
			{
				username = model.Username,
                password = model.Password,
			};
            //Console.WriteLine(model.Username);
			// Serialize the request body to JSON
			string jsonRequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
			// Create a StringContent object with the JSON content
			var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

			// Send the POST request with the request body
			HttpResponseMessage response = await client.PostAsync(apiUrl, content);
			if (response.IsSuccessStatusCode)
			{
				// Request was successful
				string responseContent = await response.Content.ReadAsStringAsync();
				// Handle the response content
				return responseContent;
			}
			else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
			{
				// Request failed, handle the error
				return "Bad";
			}
            else
            {
                return null;
            }
		}

		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			HttpContext.Session.Remove("jwtoken");
			//Console.WriteLine("Logged out");
			return Redirect("../Account/Login");
		}

		[HttpPost]
		public IActionResult SetLanguagePreference(string language)
		{
			// Save the preference in a cookie
			Response.Cookies.Append("UserLanguagePreference", language, new CookieOptions
			{
				Expires = DateTimeOffset.UtcNow.AddYears(1) // Set cookie expiration (e.g., 1 year)
			});

			return Ok(new { message = "Language preference saved." });
		}
	}
}

class Response
{
    private string Status;
    private string Message;

    public Response(string status, string message)
    {
        Status = status;
        Message = message;
    }
}
