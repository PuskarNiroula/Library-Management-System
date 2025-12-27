using Library_Management_System.Data;
using Library_Management_System.DTOs.User;
using Library_Management_System.Enum;
using Library_Management_System.Helpers;
using Library_Management_System.Models;
using Library_Management_System.Services.Admin.Exception;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Services;

public class UserService(ApplicationDbContext dbContext) : IUserService
{
    private  readonly ApplicationDbContext _context=dbContext;

    /// <summary>
    /// Creates a new student user from the provided registration data and persists it to the database.
    /// </summary>
    /// <param name="userDto">User registration data including full name, email, and plain-text password.</param>
    /// <returns>A UserResponseDto containing the newly created user's Id, FullName, Email, and Role.</returns>
    public async Task<UserResponseDto> RegisterAsync(UserDto userDto)
    {
        // Hash the password
        var hashedPassword = PasswordHelper.HashPassword(userDto.Password);

        var user = new User
        {
            FullName = userDto.FullName,
            Email = userDto.Email,
            PasswordHash = hashedPassword,
            Role=UserRoleEnum.Student
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserResponseDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role
        };
        
    }

    public async Task<bool> GetByEmailAsync(string email)
    {

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
            return true;

        return false;

    }

    /// <summary>
    /// Authenticates a user using the provided credentials and returns basic user information on success.
    /// </summary>
    /// <param name="loginDto">The user's login credentials (email and password).</param>
    /// <returns>A <see cref="UserResponseDto"/> containing the user's Id, FullName, Email, and Role when credentials are valid; `null` if the user is not found or the password is incorrect.</returns>
    public async Task<UserResponseDto> LoginAsync(LoginDto loginDto)
    {
       var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
        if (user == null)
        {
            throw new UserNotFoundException();
        }

        if (!PasswordHelper.VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            throw new UserNotFoundException();
        }
        UserResponseDto userResponse=new UserResponseDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role
        };
        return userResponse;
    }

}