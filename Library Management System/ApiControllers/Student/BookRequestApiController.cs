using Library_Management_System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management_System.ApiControllers.Student;

[Authorize(Roles = "Student")]
[ApiController]
[Route("api/student/book-request")]
public class BookRequestApiController:ControllerBase
{
    
}