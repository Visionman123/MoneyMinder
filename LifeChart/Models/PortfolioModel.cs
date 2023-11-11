namespace LifeChart.Models
{
    public class PortfolioModel
    {
        public Income? Income { get; set; }
        public Assets? Assets { get; set; }
        public Debt? Debt { get; set; } 
        public ExpenseLimit? ExpenseLimit { get; set; }       
    }

    public class Income
    {
        public decimal? Salary { get; set; }
        public List<Investment>? Investments { get; set; }
        public decimal? Others { get; set; }
    }

    public class Investment
    {
        public decimal? Amount { get; set; }
        public decimal? RoI { get; set; }

        public Investment(decimal? amount, decimal? roI)
        {
            Amount = amount;
            RoI = roI;
        }
    }

    public class Assets
    {
        public List<decimal>? RealEstates { get; set; }
        public decimal? BankAccount { get; set; }
    }

    public class Debt
    {
        public decimal? Loans { get; set; }
        public decimal? CreditCardBalance { get; set; }
    }

    public class ExpenseLimit
    {
        public decimal? Groceries { get; set; }
        public decimal? Utilities { get; set; }
        public decimal? Entertainment { get; set; }
        public decimal? Rent { get; set; }
        public decimal? Mortgages { get; set; }
        public decimal? Others { get; set; }

    }
}
