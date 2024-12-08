using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Data;
using LifeChartAPI.Controllers;

namespace LifeChartAPI.Models
{
	public class WhatIfModel
	{
		public int CurrentAge { get; set; } 
		public int FFPAge { get; set; }
		public decimal MonthlySpending { get; set; }
		public decimal Inflation { get; set; }
		public decimal BankAsset { get; set; }
		public decimal BankROI { get; set; }
		public List<FFPStage> FFPStages { get; set; }
		public decimal DefaultInflation { get; set; }	

		//init model with no attribute
		public WhatIfModel()
		{
		}

		//init model with attributes
		public WhatIfModel(string? connectionString, string? userId)
		{
			SqlConnection connection = null;
			try
			{
				FFPStages = new();
				connection = new(connectionString);
				connection.Open();
				//get UserFFP
				string sql = "SELECT CurrentAge, FFPAge, Spending, Inflation, BankAssets, RoI FROM dbo.UserFFP WHERE UserId = @UserId";
				SqlCommand cmd = new(sql, connection);
				cmd.Parameters.AddWithValue("UserId", userId);
				SqlDataReader reader = cmd.ExecuteReader();

				CurrentAge = -1;
				FFPAge = -1;
				MonthlySpending = -1;
				Inflation = -1;
				BankAsset = -1;
				BankROI = -1;
				while (reader.Read())
				{
					CurrentAge = (int) reader[0];
					FFPAge = (int)reader[1]; 
					MonthlySpending = (decimal) reader[2];
					Inflation = (decimal) reader[3] * 100;
					BankAsset = (decimal) reader[4];
					BankROI = (decimal) reader[5] * 100;
				}
				reader.Close();

				//get UserFFPStages
				sql = "SELECT Id, FromAge, ToAge, AnnualSavingIncrease, SavingPerMonth FROM dbo.UserFFPStages WHERE UserId = @UserId";
				cmd = new(sql, connection);
				cmd.Parameters.AddWithValue("@UserId", userId);
				reader = cmd.ExecuteReader();
				while (reader.Read())
				{
					FFPStage stage = new((int)reader[0], (int)reader[1], (int)reader[2], (reader[3] != DBNull.Value) ? (decimal)reader[3] * 100 : null, (reader[4] != DBNull.Value) ? (decimal)reader[4] : null);
					FFPStages.Add(stage);
				}
				reader.Close();

				//get default inflation
				sql = "SELECT InflationRate FROM dbo.InflationRates WHERE Year = @Year";
				cmd = new(sql, connection);
				cmd.Parameters.AddWithValue("@Year", DateTime.Now.Year);
				reader = cmd.ExecuteReader();
				while (reader.Read())
				{
					DefaultInflation = (decimal)reader[0];
				}
				reader.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			finally
			{
				connection?.Close();
			}
		}

		public string SaveWhatIf(string? connectionString, string? userId, int currentAge, 
			int ffpAge, double monthlySpending, double inflation, double bankAsset, double bankROI, int stages,
			int? startStage1, int? endStage1, double? annualIncreaseStage1, double? saveMonthlyStage1,
			int? startStage2, int? endStage2, double? annualIncreaseStage2, double? saveMonthlyStage2,
			int? startStage3, int? endStage3, double? annualIncreaseStage3, double? saveMonthlyStage3,
			double? saveFirstMonth, int? saveAtStage, bool? saveEnough, int? ffpAtAge)
		{
			SqlConnection connection = null;

			try
			{
				connection = new(connectionString);
				connection.Open();
				SqlCommand cmd = new("dbo.SaveWhatIf", connection);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@UserId", userId);
				cmd.Parameters.AddWithValue("@CurrentAge", currentAge);
				cmd.Parameters.AddWithValue("@FFPAge", ffpAge);
				cmd.Parameters.AddWithValue("@MonthlySpending", monthlySpending);
				cmd.Parameters.AddWithValue("@Inflation", inflation);
				cmd.Parameters.AddWithValue("@BanKAsset", bankAsset);
				cmd.Parameters.AddWithValue("@RoI", bankROI);
				cmd.Parameters.AddWithValue("@Stages", stages);
				cmd.Parameters.AddWithValue("@FromAge1", (startStage1 != null && startStage1 != 0) ? startStage1 : DBNull.Value);
				cmd.Parameters.AddWithValue("@ToAge1", (endStage1 != null && endStage1 != 0) ? endStage1 : DBNull.Value);
				cmd.Parameters.AddWithValue("@AnnualIncrease1", (annualIncreaseStage1 != null && !double.IsNaN((double) annualIncreaseStage1) ? annualIncreaseStage1: DBNull.Value));
				cmd.Parameters.AddWithValue("@Save1", (saveMonthlyStage1 != null && !double.IsNaN((double) saveMonthlyStage1) ? saveMonthlyStage1 : DBNull.Value));
				cmd.Parameters.AddWithValue("@FromAge2", (startStage2 != null && startStage2 != 0) ? startStage2 : DBNull.Value);
				cmd.Parameters.AddWithValue("@ToAge2", (endStage2 != null && endStage2 != 0) ? endStage2 : DBNull.Value);
				cmd.Parameters.AddWithValue("@AnnualIncrease2", (annualIncreaseStage2 != null && !double.IsNaN((double) annualIncreaseStage2) ? annualIncreaseStage2 : DBNull.Value));
				cmd.Parameters.AddWithValue("@Save2", (saveMonthlyStage2 != null && !double.IsNaN((double) saveMonthlyStage2) ? saveMonthlyStage2 : DBNull.Value));
				cmd.Parameters.AddWithValue("@FromAge3", (startStage3 != null && startStage3 != 0) ? startStage3 : DBNull.Value);
				cmd.Parameters.AddWithValue("@ToAge3", (endStage3 != null && endStage3 != 0) ? endStage3 : DBNull.Value);
				cmd.Parameters.AddWithValue("@AnnualIncrease3", (annualIncreaseStage3 != null && !double.IsNaN((double) annualIncreaseStage3) ? annualIncreaseStage3 : DBNull.Value));
				cmd.Parameters.AddWithValue("@Save3", (saveMonthlyStage3 != null && !double.IsNaN((double) saveMonthlyStage3) ? saveMonthlyStage3 : DBNull.Value));
				cmd.Parameters.AddWithValue("@SaveFirstMonth", (saveFirstMonth != null && !double.IsNaN((double) saveFirstMonth) ? saveFirstMonth : DBNull.Value));
				cmd.Parameters.AddWithValue("@SaveAtStage", (saveAtStage != null && saveAtStage != 0 && saveFirstMonth != 0) ? saveAtStage : DBNull.Value);
				cmd.Parameters.AddWithValue("@SaveEnough", (saveEnough != null && saveEnough == true) ? 1 : 0);
				cmd.Parameters.AddWithValue("@FFPAtAge", (ffpAtAge != null && ffpAtAge != 0) ? ffpAtAge : DBNull.Value);
				cmd.ExecuteNonQuery();

				return "200";
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return "500";
			}
			finally
			{
				connection?.Close();
			}
		}
	}


	public class FFPStage
	{
		public int Id { get; set; }
		public int FromAge { get; set; }
		public int ToAge { get; set; }
		public decimal? AnnualSavingIncrease { get; set; }
		public decimal? SavePerMonth { get; set; }

		public FFPStage(int id, int fromAge, int toAge, decimal? annualSavingIncrease, decimal? savePerMonth)
		{
			Id = id;
			FromAge = fromAge;
			ToAge = toAge;
			AnnualSavingIncrease = annualSavingIncrease;
			SavePerMonth = savePerMonth;
		}
	}
}
