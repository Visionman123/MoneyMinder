using Microsoft.Data.SqlClient;

namespace LifeChartAPI.Models
{
    public class SpendingBehaviorModel
    {
        public decimal ThreeMonthsPrior { get; set; }
        public decimal TwoMonthsPrior { get; set; }
        public decimal OneMonthPrior { get; set; }
        public decimal Limit { get; set; }

        public SpendingBehaviorModel(string? connectionString, string? userId, int? categoryId)
        {
            try
            {
                SqlConnection connection = new(connectionString);
                connection.Open();
                //get balance
                string sql = "SELECT * FROM GetExpensesThreeMonthsPriorOfCategory(@UserId, @CategoryId)";
                SqlCommand cmd = new(sql, connection);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@CategoryId", categoryId);
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

                //get name of category
                sql = "SELECT Name FROM dbo.ExpenseCategories WHERE Id = " + categoryId;
                string category = null;
                cmd = new(sql, connection);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    category = reader[0] as string;
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
