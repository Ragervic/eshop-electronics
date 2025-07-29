using Microsoft.EntityFrameworkCore;
using TestP.Models;
using TestP.Data;
using TestP.Services.Analytics;

namespace TestP.Services
{
    public class SalesAnalyticsService : ISalesAnalyticsService
    {
        private readonly ApplicationDbContext _context;
        public SalesAnalyticsService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<RevenueDataPoint>> GetRevenueTrendsAsync(DateTime startDate, DateTime endDate)
        {
            endDate = endDate.Date.AddDays(1).AddTicks(-1); // Include the entire end date

            var revenueData = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .SelectMany(o => o.OrderItems)
                .GroupBy(o => o.Order.OrderDate.Date)
                .Select(g => new RevenueDataPoint
                {
                    Date = g.Key,
                    Revenue = g.Sum(o => o.UnitPrice * o.Quantity)
                })
                .OrderBy(r => r.Date)
                .ToListAsync();

            // Ensure all dates in the range are represented
            var allDates = Enumerable.Range(0, (endDate - startDate).Days + 1)
            .Select(i => startDate.AddDays(i).Date)
            .ToList();

            var finalData = allDates.GroupJoin(revenueData,
                d => d,
                r => r.Date,
                (d, r) => new RevenueDataPoint
                {
                    Date = d,
                    Revenue = r.Sum(rp => rp.Revenue)
                }).ToList();

            return finalData;
        }

        public async Task<List<CategorySalesData>> GetSalesByCategoryAsync(DateTime startDate, DateTime endDate)
        {
            endDate = endDate.Date.AddDays(1).AddTicks(-1); // Include the entire end date

            var salesByCategory = await _context.OrderItems
                .Where(oi => oi.Order.OrderDate >= startDate && oi.Order.OrderDate <= endDate)
                .Include(oi => oi.Product)
                .ThenInclude(p => p.Category)
                .GroupBy(oi => oi.Product.Category!.Name)
                .Select(g => new CategorySalesData
                {
                    CategoryName = g.Key,
                    TotalSales = g.Sum(oi => oi.UnitPrice * oi.Quantity),
                    ProductCount = g.Select(oi => oi.ProductId).Distinct().Count()
                })
                .OrderByDescending(cs => cs.TotalSales)
                .ToListAsync();

            return salesByCategory;

        }

        public async Task<List<BestsellerProduct>> GetTopBestsellersAsync(DateTime startDate, DateTime endDate, int count)
        {
            endDate = endDate.Date.AddDays(1).AddTicks(-1);

            var topBestsellers = await _context.OrderItems
                .Where(oi => oi.Order.OrderDate >= startDate && oi.Order.OrderDate <= endDate)
                .Include(oi => oi.Product)
                .GroupBy(oi => new { oi.ProductId, oi.Product!.Name })
                .Select(g => new BestsellerProduct
                {
                    ProductName = g.Key.Name,
                    SalesCount = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(bp => bp.SalesCount)
                .Take(count) // Take the top 'count' bestsellers
                .ToListAsync();

            return topBestsellers;
        }
    }
}