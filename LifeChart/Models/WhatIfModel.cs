namespace LifeChart.Models
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

		public class FFPStage
		{
			public int Id { get; set; }
			public int FromAge { get; set; }
			public int ToAge { get; set; }
			public decimal? AnnualSavingIncrease { get; set; }
			public decimal? SavePerMonth { get; set; }
		}
	}

}
