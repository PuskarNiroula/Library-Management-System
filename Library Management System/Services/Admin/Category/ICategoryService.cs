using Library_Management_System.DTOs.Category;
using Library_Management_System.ViewModels;

namespace Library_Management_System.Services.Admin.Category;

public interface ICategoryService
{
    Task<bool> CreateCategoryAsync(CreateCategory dto);
    Task<Models.DbModels.Category?> GetCategoryByIdAsync(int categoryId);
    Task<bool> GetCategoryByNameAsync(string categoryName);
    Task<bool> DeleteCategoryByIdAsync(int categoryId);
    Task<bool> UpdateCategoryByIdAsync(UpdateCategory dto);
    Task<CategoriesViewModel> GetAllCategoriesAsync(int page);
}