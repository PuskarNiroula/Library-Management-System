
using Library_Management_System.DTOs.User;

namespace Library_Management_System.Services;

public interface IUserService
{
    Task<UserResponseDto> RegisterAsync(UserDto userDto);
   Task <bool> GetByEmailAsync(string email);
    Task<UserResponseDto?> LoginAsync(LoginDto loginDto);
    
}