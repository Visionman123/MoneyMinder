using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LifeChartAPI.Models
{
	public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{

		}

		override
		public DbSet<IdentityUser> Users
		{ get; set; }
	}
}
