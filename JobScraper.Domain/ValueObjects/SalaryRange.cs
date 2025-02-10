namespace JobScraper.Domain.ValueObjects
{
    public class SalaryRange
    {
        public decimal Min { get; private set; }
        public decimal Max { get; private set; }
        public string Currency { get; private set; }

        public SalaryRange(decimal min, decimal max, string currency)
        {
            Min = min;
            Max = max;
            Currency = currency;
        }
    }
}