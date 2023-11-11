using Microsoft.Data.SqlClient;

namespace LifeChartAPI.Models
{
    public class DashboardModel
    {
        public decimal? Balance { get; set; }
        public Dictionary<string, decimal?>? Income { get; set; } //key of format "month/year"
        public Dictionary<string, decimal?>? ExpenseList { get; set; } //key of format "month/year-category"
        public Dictionary<string, decimal?>? Expenses { get; set; } //key of format "month/year"

        public DashboardModel(string? connectionString, string? userId)
        {
            Income = new();
            ExpenseList = new();
            Expenses = new();

            try
            {
                SqlConnection connection = new(connectionString);
                connection.Open();
                //get balance
                string sql = "SELECT BankAccount FROM dbo.UserBankAccounts WHERE UserId = " + "'" + userId + "'";
                SqlCommand cmd = new(sql, connection);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Balance = reader[0] != DBNull.Value ? (decimal)reader[0] : null;
                }
                reader.Close();

                //get monthly income of 12 months
                sql = "SELECT * FROM dbo.GetMonthlyIncome(@UserId)";
                cmd = new(sql, connection);
                cmd.Parameters.AddWithValue("@UserId", userId);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    decimal salary = reader[0] != DBNull.Value ? (decimal)reader[0] : 0;
                    decimal others = reader[1] != DBNull.Value ? (decimal)reader[1] : 0;
                    decimal income = salary + others;
                    int month = (int)reader[2];
                    int year = (int)reader[3];
                    //order by month asc
                    Income.Add(month + "/" + year, income != 0 ? income : null);
                }
                reader.Close();

                //get monthly expenses of 12 months by category
                sql = "SELECT * FROM dbo.GetMonthlyExpensesByCategory(@UserId)";
                cmd = new(sql, connection);
                cmd.Parameters.AddWithValue("@UserId", userId);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string category = reader[0] as string;
                    decimal amount = (decimal)reader[1];
                    int month = (int)reader[2];
                    int year = (int)reader[3]; 
                    ExpenseList.Add(month + "/" + year + "-" + category, amount);
                }
                reader.Close();

                //get monthly expenses of 12 months 
                sql = "SELECT * FROM dbo.GetMonthlyExpenses(@UserId)";
                cmd = new(sql, connection);
                cmd.Parameters.AddWithValue("@UserId", userId);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    decimal amount = (decimal)reader[0];
                    int month = (int)reader[1];
                    int year = (int)reader[2];
                    //order by month asc
                    Expenses.Add(month + "/" + year, amount);
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
