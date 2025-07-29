using System;
using TestP.Models;
namespace TestP.Services
{
    public interface ICategoryService
    {
        Task<Category> AddCategoryAsync(Category category);
        Task<List<Category>> GetCategoriesAsync();
        Task<Category> FetchCategoryAsync(int id);

        Task<bool> EditCategoryAsync(int id);
        Task<bool> DeleteCategoryAsync(int id);

    }
}
