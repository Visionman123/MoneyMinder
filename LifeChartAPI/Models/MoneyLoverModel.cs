using Microsoft.Data.SqlClient;
using System.Data;

namespace LifeChartAPI.Models
{
    public class MoneyLoverModel
    {
        public Dictionary<string, decimal>? PastExpenses { get; set; } //key with format of "category datetime"
        public Dictionary<string, decimal>? TodayExpenses { get; set; } //key with format of "category datetime"
        public Dictionary<string, decimal>? ThisMonthExpenses { get; set; } //key with format of "category"
        public Dictionary<string, decimal>? ThisMonthLimits { get; set; } //key with format of "category"

        public decimal? GroceriesEstimation { get; set; }
        public decimal? EntertainmentEstimation { get; set; }
        public decimal? UtilitiesEstimation { get; set; }
        public decimal? OthersEstimation { get; set; }

        public MoneyLoverModel()
        {
            PastExpenses = new();
            TodayExpenses = new();
            ThisMonthExpenses = new();
            ThisMonthLimits = new();
            GroceriesEstimation = null;
            EntertainmentEstimation = null;
            UtilitiesEstimation = null;
            OthersEstimation = null;
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
                    string category = reader[1] as string;
                    DateTime dateTime =  (DateTime)reader[2];
                    decimal amount = (decimal)reader[3];
                    PastExpenses.Add(category + " " + dateTime.ToString(), amount);
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
                    string category = reader[1] as string;
                    DateTime dateTime = (DateTime)reader[2];
                    decimal amount = (decimal)reader[3];
                    TodayExpenses.Add(category + " " + dateTime.ToString(), amount);

                }
                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public string AddExpense(string? connectionString, string? userId, int? categoryId, decimal? amount, DateTime date)
        {
            try
            {
                SqlConnection connection = new(connectionString);
                connection.Open();
                SqlCommand cmd = new("dbo.AddExpense", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", userId + "-category" + categoryId + "-" +  date.ToString());
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@CategoryId", categoryId);
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
                Dictionary<int, string> categories = new();
                categories.Add(1, "groceries");
                categories.Add(2, "entertainment");
                categories.Add(3, "utilities");
                categories.Add(4, "rent");
                categories.Add(5, "mortgages"); 
                categories.Add(6, "others");
                while (reader.Read())
                {
                    int categoryId = (int)reader[0];
                    string category = reader[1] as string;
                    decimal amount = (decimal)reader[2];
                    //remove from dictionary any category found in db
                    if (category == categories[categoryId])
                    {
                        categories.Remove(categoryId);
                    }
                    ThisMonthExpenses.Add(category, amount);
                }
                //add to ThisMonthExpenses categories not in db with amount = 0
                foreach (int categoryId in categories.Keys)
                {
                    ThisMonthExpenses.Add(categories[categoryId], 0);
                }
                reader.Close();

                //get expense limits of this month 
                sql = "SELECT Groceries, Utilities, Rent, Entertainment, Mortgages, Others FROM dbo.UserMonthlyExpenseLimits WHERE UserId = " + "'" + userId + "'";
                cmd = new(sql, connection);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    decimal groceries = (reader[0] == DBNull.Value) ? 0 : (decimal)reader[0];
                    decimal utilities = (reader[1] == DBNull.Value) ? 0 : (decimal)reader[1];
                    decimal rent = (reader[2] == DBNull.Value) ? 0 : (decimal)reader[2];
                    decimal entertainment = (reader[3] == DBNull.Value) ? 0 : (decimal)reader[3];
                    decimal mortgages = (reader[4] == DBNull.Value) ? 0 : (decimal)reader[4];
                    decimal others = (reader[5] == DBNull.Value) ? 0 : (decimal)reader[5];
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
