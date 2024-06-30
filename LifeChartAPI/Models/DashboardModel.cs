using Microsoft.Data.SqlClient;
using System.Data;

namespace LifeChartAPI.Models
{
    public class DashboardModel
    {
        public List<Investment> Investments { get; set; }
        public decimal? Balance { get; set; }
        public Dictionary<string, decimal?>? Income { get; set; } //key of format "month/year"
        public Dictionary<string, decimal?>? ExpenseList { get; set; } //key of format "category"
        public Dictionary<string, decimal?>? Expenses { get; set; } //key of format "month/year"

        public DashboardModel(string? connectionString, string? userId)
        {
            Investments = new();
            Income = new();
            ExpenseList = new();
            Expenses = new();
            Balance = 0;

            try
            {
                SqlConnection connection = new(connectionString);
                connection.Open();
                //get salary, others, balance this month and updated time 
                string sql = "SELECT * FROM dbo.GetCurrentIncomeWithBalance(@UserId)";
                SqlCommand cmd = new(sql, connection);
                cmd.Parameters.AddWithValue("@UserId", userId);
                SqlDataReader reader = cmd.ExecuteReader();
                decimal salary1 = 0, others1 = 0;
                int updatedMonth = 0;
                while (reader.Read())
                {
                    salary1 += (reader[0] != DBNull.Value) ? (decimal)reader[0] : 0M;
                    others1 += (reader[1] != DBNull.Value) ? (decimal)reader[1] : 0M;
                    //current balance in db
                    Balance += (reader[2] != DBNull.Value) ? (decimal)reader[2] : 0M;
                    updatedMonth = (reader[3] != DBNull.Value) ? (int)reader[3] : 0;
                }
                if (updatedMonth == 0 || updatedMonth != System.DateTime.Now.Month)
                {
                    Balance += (salary1 + others1);
                }
                reader.Close();

				//get investments
				//sql = "SELECT Id, Amount, RoI FROM dbo.UserInvestments WHERE UserId = " + "'" + userId + "'";
				sql = "SELECT Id, Amount, RoI FROM dbo.UserInvestments WHERE UserId = @UserId";
				cmd = new(sql, connection);
				cmd.Parameters.AddWithValue("UserId", userId);
				reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id = (int)reader[0];
                    decimal amount = reader[1] != DBNull.Value ? (decimal)reader[1] : 0M;
                    decimal RoI = reader[2] != DBNull.Value ? (decimal)reader[2] : 0M;
                    Investment investment = new(id, amount, RoI);
                    Investments.Add(investment);
                    if (updatedMonth == 0 || updatedMonth != System.DateTime.Now.Month)
                    {
                        Balance += amount * (1 + RoI);
                    }
                }
                reader.Close();

                //get expenses this month
                sql = "SELECT * FROM dbo.GetCurrentExpenses(@UserId)";
                cmd = new(sql, connection);
                cmd.Parameters.AddWithValue("UserId", userId);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    decimal total = reader[0] != DBNull.Value ? (decimal)reader[0] : 0M;
                    if (updatedMonth == 0 || updatedMonth != System.DateTime.Now.Month)
                    {
                        Balance -= total;
                    }
                }
                reader.Close();


                //update balance in db
                cmd = new("dbo.UpdateBalance", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Balance", (Balance != null) ? Balance : DBNull.Value);
                cmd.Parameters.AddWithValue("@UpdatedTime", DateTime.Now);
                cmd.ExecuteNonQuery();

                ////get balance
                //sql = "SELECT BankAccount FROM dbo.UserBankAccounts WHERE UserId = " + "'" + userId + "'";
                //cmd = new(sql, connection);
                //reader = cmd.ExecuteReader();
                //while (reader.Read())
                //{
                //    Balance = reader[0] != DBNull.Value ? (decimal)reader[0] : null;
                //}
                //reader.Close();

                //get monthly income of 12 months
                sql = "SELECT * FROM dbo.GetMonthlyIncome(@UserId)";
                cmd = new(sql, connection);
                cmd.Parameters.AddWithValue("@UserId", userId);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    decimal salary = reader[0] != DBNull.Value ? (decimal)reader[0] : 0M;
                    decimal others = reader[1] != DBNull.Value ? (decimal)reader[1] : 0M;
                    decimal income = salary + others;
                    int month = (int)reader[2];
                    int year = (int)reader[3];
                    //order by month asc
                    Income.Add(month + "/" + year, income != 0 ? income : null);
                }
                reader.Close();

                //get monthly expenses of this month by category
                sql = "SELECT * FROM dbo.GetCurrentExpensesByCategory(@UserId)";
                cmd = new(sql, connection);
                cmd.Parameters.AddWithValue("@UserId", userId);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string category = reader[1] as string;
                    decimal amount = (decimal)reader[2];
                    int month = (int)reader[3];
                    int year = (int)reader[4]; 
                    ExpenseList.Add(category, amount);
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

                //Console.WriteLine("Balance: " + Balance);
                //Console.WriteLine("Income: " + Income);
                //Console.WriteLine("Expenses: " + Expenses);
                //Console.WriteLine("Expense This Month: " + ExpenseList);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
    }
}
