using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using LifeChartAPI.Models;
using LifeChartAPI.Controllers;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// db
builder.Services.AddDbContextPool<ApplicationDbContext>(
	options => options.UseSqlServer(builder.Configuration.GetConnectionString("LifeChartDatabase"))
);

//identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(
	options =>
	{
		options.SignIn.RequireConfirmedEmail = false;
		options.SignIn.RequireConfirmedPhoneNumber = false;
	}
)
		.AddRoles<IdentityRole>()
		.AddEntityFrameworkStores<ApplicationDbContext>()
		.AddDefaultTokenProviders();


builder.Services.Configure<IdentityOptions>(
	options =>
	{
		options.SignIn.RequireConfirmedEmail = true;
		options.User.RequireUniqueEmail = true;
	}
);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<AccountController>();
builder.Services.AddTransient<SpendingBehaviorController>();

//auth
builder.Services.AddAuthentication(options =>
{
}).AddJwtBearer(options =>
{
	//options.TokenValidationParameters = new TokenValidationParameters
	//{
	//    ValidateIssuer = false,
	//    ValidateAudience = false,
	//    ValidateLifetime = false,
	//    ValidateIssuerSigningKey = false,
	//    ValidIssuer = builder.Configuration["Jwt:Issuer"],
	//    ValidAudience = builder.Configuration["Jwt:Audience"],
	//    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
	//};
	options.Audience = builder.Configuration["Jwt:Audience"];
	options.Authority = builder.Configuration["Jwt:Issuer"];
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
