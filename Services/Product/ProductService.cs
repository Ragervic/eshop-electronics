using TestP.Data;
using TestP.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestP.Services;


namespace TestP.Services
{
    public class ProductService : IProductService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        public ProductService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<Product> AddProductAsync(Product product)
        {
            using var _context = _dbContextFactory.CreateDbContext();
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }
        public async Task<List<Product>> GetProductsAsync()
        {
            using var _context = _dbContextFactory.CreateDbContext();
            return await _context.Products.Include(p => p.Category).ToListAsync();
        }
        public async Task<Product> FetchProductAsync(Guid id)
        {
            using var _context = _dbContextFactory.CreateDbContext();
            var product = await _context.Products.FindAsync(id);
            if (product == null) throw new Exception("Cannot find Product!!");
            return product;
        }
        public async Task<bool> EditProductAsync(Guid id)
        {
            using var _context = _dbContextFactory.CreateDbContext();
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteProductAsync(Guid id)
        {
            using var _context = _dbContextFactory.CreateDbContext();
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}