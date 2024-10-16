using Microsoft.Data.SqlClient;
using static Plotly.NET.StyleParam.DrawingStyle;
using System.Data;

namespace LifeChartAPI.Models
{
	public class SurveyModel
	{
		public int Intake { get; set; }
		public string Major { get; set; }
		public string Gender { get; set; }
		public string FFPAchieveAge { get; set; }
		public string FFPStrategy { get; set; }

		public SurveyModel() { }

		public string SaveSurvey(string? connectionString, string? userId, int intake, string major, string gender, string ffpAchieveAge, string ffpStrategy)
		{
			SqlConnection connection = null;

			try
			{
				connection = new(connectionString);
				connection.Open();
				SqlCommand cmd = new("dbo.SaveSurvey", connection);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@UserId", userId);
				cmd.Parameters.AddWithValue("@Intake", intake);
				cmd.Parameters.AddWithValue("@Major", major);
				cmd.Parameters.AddWithValue("@Gender", gender);
				cmd.Parameters.AddWithValue("@FFPAchieveAge", ffpAchieveAge);
				cmd.Parameters.AddWithValue("@FFPStrategy", ffpStrategy);
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
