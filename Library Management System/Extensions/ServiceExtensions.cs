using Library_Management_System.Helpers;
using Library_Management_System.Services;
using Library_Management_System.Services.Admin.Book;
using Library_Management_System.Services.Admin.Category;
using Library_Management_System.Services.Admin.User;
using Library_Management_System.Services.Student;

namespace Library_Management_System.Extensions;

public static class ServiceExtensions
{
    /// <summary>
    /// Registers application services into the dependency injection container.
    /// </summary>
    /// <param name="services">The IServiceCollection to which scoped application services are added.</param>
    /// <remarks>
    /// Adds scoped registrations for: ICategoryService, IUserService, IBookService, IUserControlService, and JwtService.
    /// </remarks>
    public static void AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBookService, BookService>();
         services.AddScoped<IUserControlService,UserControlService>();
         services.AddScoped<IBookInventoryService, BookInventoryService>();
         services.AddScoped<JwtService>();
    }
}