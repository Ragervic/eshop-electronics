namespace TestP.Models
{
    // Model for Sales Analytics data
    public class SalesAnalyticsData
    {
        public List<RevenueDataPoint> RevenueTrends { get; set; } = new List<RevenueDataPoint>();
        public List<CategorySalesData> CategorySales { get; set; } = new List<CategorySalesData>();
        public List<BestsellerProduct> TopBestsellers { get; set; } = new List<BestsellerProduct>();
    }
    // Model for Revenue Trends data
    public class RevenueDataPoint
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
    }

    // Model for Sales by Category data
    public class CategorySalesData
    {
        public string? CategoryName { get; set; }
        public decimal TotalSales { get; set; }
        public int ProductCount { get; set; }
    }

    // Model for Bestseller Products
    public class BestsellerProduct
    {
        public string? ProductName { get; set; }
        public int SalesCount { get; set; }

    }

}
