using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace LifeChart.Models
{
    public class LoginModel
    {
        public string? UserID { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
    }
}