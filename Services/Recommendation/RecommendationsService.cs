using TestP.Data;
using TestP.Models;
using Microsoft.EntityFrameworkCore;

namespace TestP.Services
{
    public class RecommendationsService : IRecommendationsService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly Random _random = new Random();

        public RecommendationsService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<List<Product>> GetPersonalizedRecommendationsAsync(string userId)
        {
            var recommendations = new List<Product>();

            try
            {
                using var context = _dbContextFactory.CreateDbContext();

                // 1. Get the last product ordered by the user
                var lastOrderedProduct = await GetLastOrderedProductAsync(userId);
                if (lastOrderedProduct != null)
                {
                    recommendations.Add(lastOrderedProduct);
                }

                // 2. Get 3 products from the same category (if we have a last ordered product)
                if (lastOrderedProduct != null)
                {
                    var sameCategoryProducts = await GetProductsInSameCategoryAsync(
                        lastOrderedProduct.CategoryId, 
                        3, 
                        lastOrderedProduct.Id);
                    recommendations.AddRange(sameCategoryProducts);
                }

                // 3. Get a random product to fill up to 5 recommendations
                while (recommendations.Count < 5)
                {
                    var randomProduct = await GetRandomProductAsync(
                        recommendations.Select(p => p.Id).ToArray());
                    if (randomProduct != null && !recommendations.Any(p => p.Id == randomProduct.Id))
                    {
                        recommendations.Add(randomProduct);
                    }
                    else
                    {
                        break; // Prevent infinite loop
                    }
                }

                return recommendations;
            }
            catch (Exception ex)
            {
                // Log the error and return empty list
                Console.WriteLine($"Error getting personalized recommendations: {ex.Message}");
                return new List<Product>();
            }
        }

        public async Task<Product?> GetLastOrderedProductAsync(string userId)
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext();

                var lastOrderedProduct = await context.OrderItems
                    .Include(oi => oi.Order)
                    .Include(oi => oi.Product)
                    .ThenInclude(p => p.Category)
                    .Where(oi => oi.Order.UserId == userId)
                    .OrderByDescending(oi => oi.Order.OrderDate)
                    .Select(oi => oi.Product)
                    .FirstOrDefaultAsync();

                return lastOrderedProduct;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting last ordered product: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Product>> GetProductsInSameCategoryAsync(int categoryId, int count, Guid excludeProductId)
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext();

                var products = await context.Products
                    .Where(p => p.CategoryId == categoryId && p.Id != excludeProductId && p.StockQuantity > 0)
                    .Include(p => p.Category)
                    .OrderBy(r => Guid.NewGuid()) // Random order
                    .Take(count)
                    .ToListAsync();

                return products;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting products in same category: {ex.Message}");
                return new List<Product>();
            }
        }

        public async Task<Product?> GetRandomProductAsync(Guid excludeProductId)
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext();

                var totalProducts = await context.Products
                    .Where(p => p.Id != excludeProductId && p.StockQuantity > 0)
                    .CountAsync();

                if (totalProducts == 0) return null;

                var skip = _random.Next(0, totalProducts);
                var randomProduct = await context.Products
                    .Where(p => p.Id != excludeProductId && p.StockQuantity > 0)
                    .Include(p => p.Category)
                    .Skip(skip)
                    .FirstOrDefaultAsync();

                return randomProduct;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting random product: {ex.Message}");
                return null;
            }
        }

        public async Task<Product?> GetRandomProductAsync(Guid[] excludeProductIds)
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext();

                var totalProducts = await context.Products
                    .Where(p => !excludeProductIds.Contains(p.Id) && p.StockQuantity > 0)
                    .CountAsync();

                if (totalProducts == 0) return null;

                var skip = _random.Next(0, totalProducts);
                var randomProduct = await context.Products
                    .Where(p => !excludeProductIds.Contains(p.Id) && p.StockQuantity > 0)
                    .Include(p => p.Category)
                    .Skip(skip)
                    .FirstOrDefaultAsync();

                return randomProduct;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting random product: {ex.Message}");
                return null;
            }
        }
    }
} 