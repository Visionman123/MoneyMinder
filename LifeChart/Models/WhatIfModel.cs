namespace LifeChart.Models
{
    public class WhatIfModel
    {
        public int FinancialFreedomPoint { get; set; }
        public float Spending { get; set; }
        public float Inflation { get; set; }

        public WhatIfModel(int financialFreedomPoint, float spending, float inflation)
        {
            FinancialFreedomPoint = financialFreedomPoint;
            Spending = spending;
            Inflation = inflation;
        }
    }

}
