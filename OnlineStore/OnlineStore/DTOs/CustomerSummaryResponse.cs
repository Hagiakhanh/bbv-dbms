namespace OnlineStore.DTOs
{
    public class CustomerSummaryResponse
    {
        public int TotalCustomers { get; set; }
        public double TotalCustomersGrowthPercentage { get; set; }

        public int MembersCount { get; set; }
        public double MembersGrowthPercentage { get; set; }

        public int ActiveNowCount { get; set; }
        public double ActiveNowGrowthPercentage { get; set; }
    }
}
