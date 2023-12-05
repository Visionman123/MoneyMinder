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
        public int? Id { get; set; }
        public decimal? Amount { get; set; }
        public decimal? RoI { get; set; }

        public Investment(int? id, decimal? amount, decimal? roI)
        {
            Id = id;
            Amount = amount;
            RoI = roI;
        }
    }

    public class Assets
    {
        public List<RealEstate>? RealEstates { get; set; }
        public decimal? BankAccount { get; set; }
    }

    public class RealEstate
    {
        public int? Id { get; set; }
        public decimal? Amount { get; set; }

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
