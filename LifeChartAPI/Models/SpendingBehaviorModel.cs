using Microsoft.Data.SqlClient;

namespace LifeChartAPI.Models
{
    public class SpendingBehaviorModel
    {
        public decimal ThreeMonthsPrior { get; set; }
        public decimal TwoMonthsPrior { get; set; }
        public decimal OneMonthPrior { get; set; }
        public decimal Limit { get; set; }

        public SpendingBehaviorModel(string? connectionString, string? userId, string? category)
        {
            try
            {
                SqlConnection connection = new(connectionString);
                connection.Open();
                //get balance
                string sql = "SELECT * FROM GetExpensesThreeMonthsPriorOfCategory(@UserId, @Category)";
                SqlCommand cmd = new(sql, connection);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Category", category);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int month = (int)reader[0];
                    if (month == DateTime.Now.Month - 1)
                    {
                        OneMonthPrior = reader[1] != DBNull.Value ? (decimal)reader[1] : 0;

                    }
                    else if (month == DateTime.Now.Month - 2)
                    {
                        TwoMonthsPrior = reader[1] != DBNull.Value ? (decimal)reader[1] : 0;
                    }
                    else if (month == DateTime.Now.Month - 3)
                    {
                        ThreeMonthsPrior = reader[1] != DBNull.Value ? (decimal)reader[1] : 0;
                    }
                }
                reader.Close();

                //get limit of category
                string categoryUpper = char.ToUpper(category[0]) + category.Substring(1);
                sql = "SELECT " + categoryUpper + " FROM dbo.UserMonthlyExpenseLimits WHERE UserId = " + "'" + userId + "'";
                cmd = new(sql, connection);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Limit = reader[0] != DBNull.Value ? (decimal)reader[0] : 0;
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
