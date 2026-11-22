using Library_Management_System.Data;
using Library_Management_System.DTOs.Category;
using Library_Management_System.ViewModels;
using Microsoft.EntityFrameworkCore;


namespace Library_Management_System.Services.Admin.Category;

public class CategoryService(ApplicationDbContext dbContext) : ICategoryService
{
   
    
    /// <summary>
    /// Creates a new category from the provided DTO and persists it to the database.
    /// </summary>
    /// <param name="dto">DTO containing the CategoryName for the new category.</param>
    /// <returns>`true` if the category was saved to the database, `false` otherwise.</returns>
    public async Task<bool> CreateCategoryAsync(CreateCategory dto)
    {
        Models.DbModels.Category category = new Models.DbModels.Category
        {
            CategoryName = dto.CategoryName,
        };
        await dbContext.Category.AddAsync(category);
        return await dbContext.SaveChangesAsync() > 0;
    }

    public async Task<Models.DbModels.Category?> GetCategoryByIdAsync(int categoryId)
    {
        Models.DbModels.Category? category = await dbContext.Category.FindAsync(categoryId);
        return category ?? null;
    }
    /// <summary>
    /// Determines whether a category with the specified name exists.
    /// </summary>
    /// <param name="categoryName">The category name to match.</param>
    /// <returns>`true` if a category with the specified name exists, `false` otherwise.</returns>
    public async Task<bool> GetCategoryByNameAsync(string categoryName)
    {
        return await dbContext.Category
            .AnyAsync(c => c.CategoryName == categoryName);
    }

    /// <summary>
    /// Deletes the category with the specified ID from the database.
    /// </summary>
    /// <param name="categoryId">The primary key of the category to delete.</param>
    /// <returns>`true` if a category was found and deleted, `false` if no category with the given ID exists.</returns>
    public async Task<bool> DeleteCategoryByIdAsync(int categoryId)
    {
        var category =await GetCategoryByIdAsync(categoryId);
        if (category == null) return false;
        dbContext.Remove(category);
        await dbContext.SaveChangesAsync();
        return true;

    }

    /// <summary>
    /// Updates the name of an existing category identified by the DTO's CategoryId.
    /// </summary>
    /// <param name="dto">An UpdateCategory DTO containing the target CategoryId and the new CategoryName.</param>
    /// <returns>`true` if the category was found and updated, `false` if no category with the specified ID exists.</returns>
    public async Task<bool> UpdateCategoryByIdAsync(UpdateCategory dto)
    {
        var category = await GetCategoryByIdAsync(dto.CategoryId);
        if(category==null) return false;
        category.CategoryName = dto.CategoryName;
        await dbContext.SaveChangesAsync();
        return true;
    }
    

    public async Task<CategoriesViewModel> GetAllCategoriesAsync(int page=1)
    {
        const int pageSize = 10;
        var query= dbContext.Category
            .AsNoTracking()
            .OrderBy(b => b.CategoryName);

        int totalCategories = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCategories/(double)pageSize);
        
        page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));
        var categories = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var vm = new CategoriesViewModel
        {
            Categories = categories,
            CurrentPage = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };
        return vm;

    }
}