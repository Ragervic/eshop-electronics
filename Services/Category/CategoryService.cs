using TestP.Data;
using TestP.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestP.Services;


namespace TestP.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        public CategoryService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<Category> AddCategoryAsync(Category category)
        {
            using var _context = _dbContextFactory.CreateDbContext();
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            using var _context = _dbContextFactory.CreateDbContext();
            return await _context.Categories
            .Select(c => new Category
            {
                Id = c.Id,
                Name = c.Name,
                ProductCount = c.Products.Count()
            })
            .OrderBy(c => c.Name)
            .ToListAsync();
        }
        public async Task<Category> FetchCategoryAsync(int id)
        {
            using var _context = _dbContextFactory.CreateDbContext();
            var category = await _context.Categories.FindAsync(id);
            if (category == null) throw new Exception("Cannot Find Category!!");
            return category;

        }
        public async Task<bool> EditCategoryAsync(int id)
        {
            using var _context = _dbContextFactory.CreateDbContext();
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteCategoryAsync(int id)
        {
            using var _context = _dbContextFactory.CreateDbContext();
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
