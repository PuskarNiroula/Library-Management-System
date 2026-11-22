using Library_Management_System.DTOs.Category;

namespace Library_Management_System.Services.Admin.Category;

public interface ICategoryService
{
    Task<bool> CreateCategoryAsync(CreateCategory dto);
    Task<Models.DbModels.Category?> GetCategoryByIdAsync(int categoryId);
    Task<bool> GetCategoryByNameAsync(string categoryName);
    Task<bool> DeleteCategoryByIdAsync(int categoryId);
    Task<bool> UpdateCategoryByIdAsync(UpdateCategory dto);
    Task<List<Models.DbModels.Category>> GetAllCategoriesAsync();
}