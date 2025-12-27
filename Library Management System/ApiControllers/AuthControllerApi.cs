using Library_Management_System.DTOs.User;
using Library_Management_System.Helpers;
using Library_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management_System.ApiControllers;

[ApiController]
[Route("api/auth")]
public class AuthControllerApi(IUserService userService,JwtService jwtService) : ControllerBase
{
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    private readonly JwtService _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));


    /// <summary>
    /// Registers a new user account using the provided user data.
    /// </summary>
    /// <param name="dto">User data transfer object containing registration fields (including Password and ConfirmPassword).</param>
    /// <returns>
    /// An IActionResult representing the operation result:
    /// - 400 Bad Request when the request body is invalid, the passwords do not match, or the email is already registered;
    /// - 200 OK with a success message when registration completes.
    /// </returns>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] UserDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                status = "error",
                message = "Invalid request body"
            });
        }

        if (dto.Password != dto.ConfirmPassword)
        {
            return BadRequest(new
            {
                status = "error",
                message = "Passwords do not match"
            });
        }
        if (!await _userService.GetByEmailAsync(dto.Email))
            return BadRequest(new
            {
                status = "error",
                message = "User already exists"
            });
        
        await _userService.RegisterAsync(dto);
        return Ok(new
        {
            status = "success",
            message = "Registration successful"
        });
    }
    /// <summary>
    /// Authenticates a user and, on success, issues a JWT and stores it in an HTTP-only secure cookie.
    /// </summary>
    /// <param name="dto">Login credentials (typically email/username and password).</param>
    /// <returns>
    /// An IActionResult containing:
    /// - 200 OK with { status = "success", message = "Login successful", role = user.Role } and a cookie named "jwt_token" (HttpOnly, Secure, SameSite=Strict, expires in 4 hours) when authentication succeeds;
    /// - 400 Bad Request with { status = "Invalid", message = "Invalid request body" } when the request model is invalid;
    /// - 400 Bad Request with { status = "error", message = "Invalid credentials" } when authentication fails.
    /// </returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAsync([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                status = "Invalid",
                message = "Invalid request body"
            });
        }

        try
        {
            var user = await _userService.LoginAsync(dto);
            
            // Generate JWT token
            var token = _jwtService.GenerateToken(user.Id.ToString(), user.Role,user.FullName);
            Response.Cookies.Append("jwt_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(4)
            });

            return Ok(new
            {
                status = "success",
                message = "Login successful",
                role = user.Role,
            });

        }catch(Exception ex)
        {
            return BadRequest(new
                {
                    status = "error",
                    message = "Login failed "+ex.Message,
                }
            );
        }

      
    }
    
        /// <summary>
        /// Removes the authentication cookie named "jwt" from the response if present and returns a logout confirmation.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> with HTTP 200 OK and a JSON object containing the message "Logged out successfully".</returns>
        [HttpPost("logout")]
        [Authorize] 
        public IActionResult Logout()
        {
          
            if (Request.Cookies.ContainsKey("jwt_token"))
            {
                Response.Cookies.Delete("jwt_token");
            }
            return Ok(new { message = "Logged out successfully" });
        }
}



