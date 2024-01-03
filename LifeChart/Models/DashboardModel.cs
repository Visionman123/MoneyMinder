namespace LifeChart.Models
{
    public class DashboardModel
    {
        public List<Investment> Investments { get; set; }
        public decimal? Balance { get; set; }
        public Dictionary<string, decimal?>? Income { get; set; } //key of format "month/year"
        public Dictionary<string, decimal?>? ExpenseList { get; set; } //key of format "category"
        public Dictionary<string, decimal?> Expenses { get; set; } //key of format "month/year"

        public DashboardModel(decimal? balance, List<Investment> investments, Dictionary<string, decimal?>? income, Dictionary<string, decimal?>? expenseList, Dictionary<string, decimal?> expenses)
        {
            Balance = balance;
            Investments = investments;
            Income = income;
            ExpenseList = expenseList;
            Expenses = expenses;
        }
    }
}
