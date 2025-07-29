using TestP.Models;

namespace TestP.Services.Analytics
{
    public interface ISalesAnalyticsService
    {
        Task<List<RevenueDataPoint>> GetRevenueTrendsAsync(DateTime startDate, DateTime endDate);
        Task<List<CategorySalesData>> GetSalesByCategoryAsync(DateTime startDate, DateTime endDate);
        Task<List<BestsellerProduct>> GetTopBestsellersAsync(DateTime startDate, DateTime endDate, int count);
    }

}

