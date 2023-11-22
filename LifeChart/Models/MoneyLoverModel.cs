namespace LifeChart.Models
{
    public class MoneyLoverModel
    {
        //public string? Category { get; set; }
        //public decimal? Amount { get ;set; }
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
    }
}