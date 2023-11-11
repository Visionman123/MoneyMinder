using Microsoft.Data.SqlClient;
using System.Data;

namespace LifeChartAPI.Models
{
    public class MoneyLoverModel
    {
        public Dictionary<string, decimal>? PastExpenses { get; set; } //key with format of "category"
        public Dictionary<string, decimal>? TodayExpenses { get; set; } //key with format of "category"
        public Dictionary<string, decimal>? ThisMonthExpenses { get; set; } //key with format of "category"
        public Dictionary<string, decimal>? ThisMonthLimits { get; set; } //key with format of "category"

        public decimal? GroceriesEstimation { get; set; }
        public decimal? EntertainmentEstimation { get; set; }

        public MoneyLoverModel()
        {
            PastExpenses = new();
            TodayExpenses = new();
            ThisMonthExpenses = new();
            ThisMonthLimits = new();
            GroceriesEstimation = null;
            EntertainmentEstimation = null;
        }

        public void GetPastExpenses(string? connectionString, string? userId, DateTime date)
        {
            try
            {
                SqlConnection connection = new(connectionString);
                connection.Open();
                //get expenses of chosen date
                string sql = "SELECT * FROM dbo.GetExpensesOfChosenDateByCategory(@UserId, @Date)";
                SqlCommand cmd = new(sql, connection);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Date", date);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string category = reader[0] as string;
                    decimal amount = (decimal)reader[1];
                    PastExpenses.Add(category, amount);
                }
                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void GetTodayExpenses(string? connectionString, string? userId)
        {
            try
            {
                SqlConnection connection = new(connectionString);
                connection.Open();
                //get today expenses
                string sql = "SELECT * FROM dbo.GetTodayExpensesByCategory(@UserId)";
                SqlCommand cmd = new(sql, connection);
                cmd.Parameters.AddWithValue("@UserId", userId);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string category = reader[0] as string;
                    decimal amount = (decimal)reader[1];
                    TodayExpenses.Add(category, amount);

                }
                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public string AddExpense(string? connectionString, string? userId, string? category, decimal? amount, DateTime date)
        {
            try
            {
                SqlConnection connection = new(connectionString);
                connection.Open();
                SqlCommand cmd = new("dbo.AddExpense", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", userId + "-" + category + "-" +  date.ToString());
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Category", category);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@Date", date);
                cmd.ExecuteNonQuery();

                connection.Close();
                return "200";

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return "500";
            }
        }

        public void GetExpenseTracker(string? connectionString, string? userId)
        {
            try
            {
                SqlConnection connection = new(connectionString);
                connection.Open();
                //get expenses of this month so far
                string sql = "SELECT * FROM dbo.GetCumulativeExpensesByCategory(@UserId)";
                SqlCommand cmd = new(sql, connection);
                cmd.Parameters.AddWithValue("@UserId", userId);
                SqlDataReader reader = cmd.ExecuteReader();

                //all categories
                Dictionary<string, string> categories = new();
                categories.Add("groceries", "groceries");
                categories.Add("entertainment", "entertainment");
                categories.Add("utilities", "utilities");
                categories.Add("rent", "rent");
                categories.Add("mortgages", "mortgages"); 
                categories.Add("others", "others");
                while (reader.Read())
                {
                    string category = reader[0] as string;
                    decimal amount = (decimal)reader[1];
                    //remove from dictionary any category found in db
                    if (category == categories[category])
                    {
                        categories.Remove(category);
                    }
                    ThisMonthExpenses.Add(category, amount);
                }
                //add to ThisMonthExpenses categories not in db with amount = 0
                foreach (string category in categories.Keys)
                {
                    ThisMonthExpenses.Add(category, 0);
                }
                reader.Close();

                //get expense limits of this month 
                sql = "SELECT Groceries, Utilities, Rent, Entertainment, Mortgages, Others FROM dbo.UserMonthlyExpenseLimits WHERE UserId = " + "'" + userId + "'";
                cmd = new(sql, connection);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    decimal groceries = (decimal)reader[0];
                    decimal utilities = (decimal)reader[1];
                    decimal rent = (decimal)reader[2];
                    decimal entertainment = (decimal)reader[3];
                    decimal mortgages = (decimal)reader[4];
                    decimal others = (decimal)reader[5];
                    ThisMonthLimits.Add("groceries", groceries);
                    ThisMonthLimits.Add("utilities", utilities);
                    ThisMonthLimits.Add("rent", rent);
                    ThisMonthLimits.Add("entertainment", entertainment);
                    ThisMonthLimits.Add("mortgages", mortgages);
                    ThisMonthLimits.Add("others", others);
                }
                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
