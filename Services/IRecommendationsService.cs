using TestP.Models;

namespace TestP.Services
{
    public interface IRecommendationsService
    {
        Task<List<Product>> GetPersonalizedRecommendationsAsync(string userId);
        Task<Product?> GetLastOrderedProductAsync(string userId);
        Task<List<Product>> GetProductsInSameCategoryAsync(int categoryId, int count, Guid excludeProductId);
        Task<Product?> GetRandomProductAsync(Guid excludeProductId);
    }
} 