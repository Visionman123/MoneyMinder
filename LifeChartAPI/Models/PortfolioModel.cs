using Microsoft.Data.SqlClient;
using System.Data;

namespace LifeChartAPI.Models
{
    public class PortfolioModel
    {
        public Income? Income { get; set; }
        public Assets? Assets { get; set; }
        public Debt? Debt { get; set; }
        public ExpenseLimit? ExpenseLimit { get; set; }

        public DateTime Date { get; set; }

        private SqlConnection? Connection { get; set; }

        //init model with no attribute
        public PortfolioModel()
        {
            Income = new();
            Assets = new();
            Debt = new();
            ExpenseLimit = new();
        }
        
        //init model with attributes
        public PortfolioModel(string? connectionString, string? userId)
        {
            try
            {
                //establish connection (reusable)
                Connection = new SqlConnection(connectionString);
                Income = new(Connection, userId);
                Assets = new(Connection, userId);
                Debt = new(Connection, userId);
                ExpenseLimit = new(Connection, userId);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public string EditPortfolio(string? connectionString, string? userId, Income? income, Assets? assets, Debt? debt, ExpenseLimit? expenseLimit, DateTime? date)
        {
            try
            {
                SqlConnection connection = new(connectionString);
                connection.Open();
                SqlCommand cmd = new("dbo.EditIncome", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Salary", (income.Salary != null) ? income.Salary : DBNull.Value);
                cmd.Parameters.AddWithValue("@Others", (income.Others != null) ? income.Others : DBNull.Value);
                cmd.Parameters.AddWithValue("@Date", date);
                cmd.ExecuteNonQuery();

                cmd = new("dbo.EditInvestment", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                string investmentIds = "", investmentAmounts = "", investmentRois = "";
                foreach (var investment in income.Investments)
				{
                    investmentIds += investment.Id + ",";
                    investmentAmounts += investment.Amount + ",";
                    investmentRois += investment.RoI + ",";
				}
                //remove last comma
                if (income.Investments.Count > 0)
                {
                    investmentIds = investmentIds.Remove(investmentIds.Length - 1);
                    investmentAmounts = investmentAmounts.Remove(investmentAmounts.Length - 1);
                    investmentRois = investmentRois.Remove(investmentRois.Length - 1);
                }
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@InvestmentIds", investmentIds);
                cmd.Parameters.AddWithValue("@Amounts", investmentAmounts);
                cmd.Parameters.AddWithValue("@RoIs", investmentRois);
                cmd.ExecuteNonQuery();


                cmd = new("dbo.EditBankAccount", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@BankAccount", (assets.BankAccount != null) ? assets.BankAccount : DBNull.Value);
                cmd.ExecuteNonQuery();


                cmd = new("dbo.EditRealEstates", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                string estateIds = "", estateAmounts = "";
                foreach (var estate in assets.RealEstates)
                {
                    //Console.WriteLine(estate.Amount);
                    estateIds += estate.Id + ",";
                    estateAmounts += estate.Amount + ",";
                }
                //remove last comma
                if (assets.RealEstates.Count > 0)
                {
                    estateIds = estateIds.Remove(estateIds.Length - 1);
                    estateAmounts = estateAmounts.Remove(estateAmounts.Length - 1);
                }
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@EstateIds", estateIds);
                cmd.Parameters.AddWithValue("@Amounts", estateAmounts);
                cmd.ExecuteNonQuery();



                cmd = new("dbo.EditDebts", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Loans", (debt.Loans != null) ? debt.Loans : DBNull.Value);
                cmd.Parameters.AddWithValue("@CreditCardBalance", (debt.CreditCardBalance != null) ? debt.CreditCardBalance : DBNull.Value);
                cmd.ExecuteNonQuery();
                     
                cmd = new("dbo.EditMonthlyExpenseLimits", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Groceries", (expenseLimit.Groceries != null) ? expenseLimit.Groceries : DBNull.Value);
                cmd.Parameters.AddWithValue("@Utilities", (expenseLimit.Utilities != null) ? expenseLimit.Utilities : DBNull.Value);
                cmd.Parameters.AddWithValue("@Entertainment", (expenseLimit.Entertainment != null) ? expenseLimit.Entertainment : DBNull.Value);
                cmd.Parameters.AddWithValue("@Rent",(expenseLimit.Rent != null) ? expenseLimit.Rent : DBNull.Value);
                cmd.Parameters.AddWithValue("@Mortgages",(expenseLimit.Mortgages != null) ? expenseLimit.Mortgages : DBNull.Value);
                cmd.Parameters.AddWithValue("@Others", (expenseLimit.Others != null) ? expenseLimit.Others : DBNull.Value);
                cmd.ExecuteNonQuery();

                connection.Close();
                return "200";
            }

            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return "500";
            }
        }

        public string DeleteInvestment(string? connectionString, string? userId, int? investmentId)
        {
            try
            {
                SqlConnection connection = new(connectionString);
                connection.Open();
                SqlCommand cmd = new("dbo.DeleteInvestment", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@InvestmentId", investmentId);
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

        public string DeleteRealEstate(string? connectionString, string? userId, int? estateId)
        {
            try
            {
                SqlConnection connection = new(connectionString);
                connection.Open();
                SqlCommand cmd = new("dbo.DeleteRealEstate", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@EstateId", estateId);
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
    }

    public class Income
    {
        public decimal? Salary { get; set; } 
        public List<Investment>? Investments { get; set; }
        public decimal? Others { get; set; }
      
        public Income()
        {
            Salary = null;
            Investments = new();
            Others = null;
        }
        public Income(SqlConnection connection, string? userId)
        {
            Investments = new();

            try
            {
                connection.Open();
                //get investments
                string sql = "SELECT Id, Amount, RoI FROM dbo.UserInvestments WHERE UserId = " + "'" + userId + "'";
                SqlCommand cmd = new(sql, connection);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Investment investment = new((int)reader[0], (decimal)reader[1], (decimal)reader[2]);
                    Investments.Add(investment);
                }
                reader.Close();

                //get current salary and others
                sql = "SELECT * FROM dbo.GetCurrentIncome(@UserId)";
                cmd = new(sql, connection);
                cmd.Parameters.AddWithValue("@UserId", userId);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Salary = (reader[0] != DBNull.Value) ? (decimal)reader[0] : null;
                    Others = (reader[1] != DBNull.Value) ? (decimal)reader[1] : null;
                    
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

    public class Investment
    {
        public int? Id { get; set; }
        public decimal? Amount { get; set; }
        public decimal? RoI { get; set; }


        public Investment()
        {
            Id = null;
            Amount = null;
            RoI = null;
        }
        public Investment(int? id, decimal? amount, decimal? roi)
        {
            Id = id;
            Amount = amount;
            RoI = roi;
        }
    }

    public class Assets
    {
        public List<RealEstate>? RealEstates { get; set; }
        public decimal? BankAccount { get; set; }


        public Assets()
        {
            RealEstates = new();
            BankAccount = null;
        }

        public Assets(SqlConnection connection, string? userId)
        {
            RealEstates = new();

            try
            {
                connection.Open();
                //get bank account
                string sql = "SELECT BankAccount FROM dbo.UserBankAccounts WHERE UserId = " + "'" + userId + "'";
                SqlCommand cmd = new(sql, connection);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    BankAccount = (reader[0] != DBNull.Value) ? (decimal)reader[0] : null;
                }
                reader.Close();

                //get real estates
                sql = "SELECT Id, Amount FROM dbo.UserRealEstates WHERE UserId = " + "'" + userId + "'";
                cmd = new(sql, connection);
                cmd.Parameters.AddWithValue("@UserId", userId);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RealEstate realEstate = new((int)reader[0], (decimal)reader[1]);
                    RealEstates.Add(realEstate);
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

    public class RealEstate
    {
        public int? Id { get; set; }
        public decimal? Amount { get; set; }

        public RealEstate()
        {
            Id = null;
            Amount = null;
        }
        public RealEstate(int? id, decimal? amount)
        {
            Id = id;
            Amount = amount;
        }
    }

    public class Debt
    {
        public decimal? Loans { get; set; }
        public decimal? CreditCardBalance { get; set; }

        public Debt() 
        {
            Loans = null;
            CreditCardBalance = null;
        }

        public Debt(SqlConnection connection, string? userId)
        {

            try
            {
                connection.Open();
                //get bank account
                string sql = "SELECT Loans, CreditCardBalance FROM dbo.UserDebts WHERE UserId = " + "'" + userId + "'";
                SqlCommand cmd = new(sql, connection);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Loans = (reader[0] != DBNull.Value) ? (decimal)reader[0] : null;
                    CreditCardBalance = (reader[1] != DBNull.Value) ? (decimal)reader[1] : null;
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

    public class ExpenseLimit
    {
        public decimal? Groceries { get; set; }
        public decimal? Utilities { get; set; }
        public decimal? Entertainment { get; set; }
        public decimal? Rent { get; set; }
        public decimal? Mortgages { get; set; }
        public decimal? Others { get; set; }


        public ExpenseLimit()
        {
            Groceries = null;
            Utilities = null;
            Entertainment = null;
            Rent = null;
            Mortgages = null;
            Others = null;
        }

        public ExpenseLimit(SqlConnection connection, string? userId)
        {
            try
            {
                //SqlConnection connection = new(connectionString);
                connection.Open();
                //get bank account
                string sql = "SELECT Groceries, Utilities, Rent, Entertainment, Mortgages, Others FROM dbo.UserMonthlyExpenseLimits WHERE UserId = " + "'" + userId + "'";
                SqlCommand cmd = new(sql, connection);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Groceries = (reader[0] != DBNull.Value) ? (decimal)reader[0] : null;
                    Utilities = (reader[1] != DBNull.Value) ? (decimal)reader[1] : null;
                    Rent = (reader[2] != DBNull.Value) ? (decimal)reader[2] : null;
                    Entertainment = (reader[3] != DBNull.Value) ? (decimal)reader[3] : null;
                    Mortgages = (reader[4] != DBNull.Value) ? (decimal)reader[4] : null;
                    Others = (reader[5] != DBNull.Value) ? (decimal)reader[5] : null;
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
