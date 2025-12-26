using Library_Management_System.DTOs.Category;
using Library_Management_System.Enum;
using Library_Management_System.Services.Admin.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management_System.ApiControllers.Admin;
[Route("api/category-api/[action]/{id?}")]
[ApiController]
[AutoValidateAntiforgeryToken]
[Authorize(Roles =UserRoleEnum.Admin)]
public class CategoryApiController(ICategoryService service) : ControllerBase
{
    private readonly ICategoryService _service = service ?? throw new ArgumentNullException(nameof(service));
    public IActionResult? Index()
    {
        return null;
    }

    /// <summary>
    /// Creates a new category using the provided DTO.
    /// </summary>
    /// <param name="dto">DTO containing the category data; <see cref="CreateCategory.CategoryName"/> must be a non-empty string.</param>
    /// <returns>An <see cref="IActionResult"/> representing the HTTP response: `BadRequest` if the category name is empty; `Conflict` if a category with the same name already exists; `Ok` when creation succeeds; `StatusCode(401)` on internal failure.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody]CreateCategory dto)
    {
        if (string.IsNullOrWhiteSpace(dto.CategoryName))
        {
            return BadRequest(new
            {
                status = "error",
                message = "Category name cannot be empty"

            });
        }

        if (await _service.GetCategoryByNameAsync(dto.CategoryName))
            return Conflict( new
            {
                status = "error",
                message = "Category already exists"
            });
        
        if (await _service.CreateCategoryAsync(dto))
        {
            return Ok(new
            {
                status = "success",
                message = "Created Successfully"
            });
        }

        return StatusCode(401, new
        {
            status = "error",
            message = "Internal Server Error just to check"
        });

    }

    /// <summary>
    /// Deletes the category identified by the specified id.
    /// </summary>
    /// <param name="id">The identifier of the category to delete.</param>
    /// <returns>`200 OK` with a success payload when deletion succeeds; `400 Bad Request` with an error payload if the category does not exist; `500 Internal Server Error` with an error payload if deletion fails.</returns>
    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        if ( await _service.GetCategoryByIdAsync(id)==null)
            return BadRequest(new
            {
                status = "error",
                message = "Category does not exist"
            });
        if (await _service.DeleteCategoryByIdAsync(id))
        {
            return Ok(new
            {
                status = "success",
                message = "Deleted Successfully"
            });
        }
        return StatusCode(500, new
        {
            status = "error",
            message = "Internal Server Error just to check"
        });
    }

    /// <summary>
    /// Updates an existing category using the values in the provided UpdateCategory DTO.
    /// </summary>
    /// <param name="dto">Update payload containing the category identifier and the new values (including CategoryName).</param>
    /// <returns>An IActionResult representing the outcome: `Conflict` with an error message if a category with the requested name already exists; `Ok` with a success message when the update succeeds; `500` with an error message for other failures.</returns>
    [HttpPut]
    public async Task<IActionResult> Update(UpdateCategory dto)
    {
        if (await _service.GetCategoryByNameAsync(dto.CategoryName))
            return Conflict(new
            {
                status = "error",
                message = "Category already exists"
            });

        if (await _service.UpdateCategoryByIdAsync(dto))
            return Ok(new
            {
                status = "success",
                message = "Updated Successfully"
            });
        
        return StatusCode(500, new
        {
            status = "error",
            message = "Internal Server Error"
        });
    }
        
    }