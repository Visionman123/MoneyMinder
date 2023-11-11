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
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        //login
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await AuthenticateUser(model);

                if (user != null)
                {
                    var tokenString = await GetJWT(user);
                    //Save token in session object
                    if (tokenString != null)
                    {
                        HttpContext.Session.SetString("jwtoken", tokenString);
                        Console.WriteLine(HttpContext.Session.GetString("jwtoken"));
                        return Redirect("../Home/Dashboard");
                    }
                    else
                    {
                        return View();
                    }
                }
            }
            ViewBag.LoginErrorMessage = "Wrong username or password";
            return View();
        }

        private async Task<string> GetJWT(IdentityUser user)
        {
            var client = new HttpClient();
            // Define the URL of the API endpoint where you want to send the POST request
            string apiUrl = "https://localhost:7147/api/TokenGeneration";

            // Define the content you want to include in the request body (e.g., a JSON object)
            var requestBody = new
            {
                UID = user.Id,
            };

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
            else
            {
                // Request failed, handle the error
                return null;
            }
        }

        private async Task<IdentityUser> AuthenticateUser(LoginModel model)
        {
            IdentityUser user = null;
            string username = "", password = "";

            username = model.Username!;
            password = model.Password!;
            var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                //get user in db
                user = await _userManager.FindByNameAsync(username);
            }
            return user;
        }


        //register
        [HttpPost]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            string email = "", username = "", password = "", rePassword = "", phoneNumber = "";
            if (ModelState.IsValid)
            {
                email = model.Email!;
                username = model.Username!;
                password = model.Password!;
                rePassword = model.RePassword!;
                phoneNumber = model.PhoneNumber!;
            }
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                ViewBag.RegistrationResultMessage = "User already exists";

            }

            else if (password != rePassword)
            {
                ViewBag.RegistrationResultMessage = "Passwords do not match";
            }

            else
            {
                var newUser = new IdentityUser
                {
                    UserName = username,
                    Email = email,
                    PhoneNumber = phoneNumber
                };
                var result = await _userManager.CreateAsync(newUser, password);
                if (result.Succeeded)
                {
                    //add token
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                    var confirmLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email = newUser.Email }, Request.Scheme);
                    var message = new Message(new string[] { newUser.Email }, "Confirmation Link", confirmLink!);
                    _emailService.SendEmail(message);
                    ViewBag.RegistrationResultMessage = "A confirmation email has been sent to your mailbox";

                }
                else
                {
                    ViewBag.RegistrationResultMessage = "Failed to register account";
                }
            }
            return View();
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return Redirect("Account/Login");
                }
            }
            return StatusCode(StatusCodes.Status500InternalServerError,
                new Response("Error", "User does not exist")
            );
        }

        // logout
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("../Account/Login");
        }
    }
}

public class Response
{
    public string Status;
    public string Message;

    public Response(string status, string message)
    {
        Status = status;
        Message = message;
    }
}
