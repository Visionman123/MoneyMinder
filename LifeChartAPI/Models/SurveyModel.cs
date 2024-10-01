using Microsoft.Data.SqlClient;
using static Plotly.NET.StyleParam.DrawingStyle;
using System.Data;

namespace LifeChartAPI.Models
{
	public class SurveyModel
	{
		public string Name { get; set; }
		public int Age { get; set; }
		public string Gender { get; set; }

		public SurveyModel() { }

		public string SaveSurvey(string? connectionString, string? userId, string name, int age, string gender)
		{
			SqlConnection connection = null;

			try
			{
				connection = new(connectionString);
				connection.Open();
				SqlCommand cmd = new("dbo.SaveSurvey", connection);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@UserId", userId);
				cmd.Parameters.AddWithValue("@Name", name);
				cmd.Parameters.AddWithValue("@Age", age);
				cmd.Parameters.AddWithValue("@Gender", gender);
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
}
