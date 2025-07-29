using System;
using TestP.Models;
namespace TestP.Services
{
    public interface IProductService
    {
        Task<Product> AddProductAsync(Product product);
        Task<List<Product>> GetProductsAsync();
        Task<Product> FetchProductAsync(Guid id);

        Task<bool> EditProductAsync(Guid id);
        Task<bool> DeleteProductAsync(Guid id);

    }
}
