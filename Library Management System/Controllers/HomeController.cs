using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management_System.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    /// <summary>
    /// Displays the login view.
    /// </summary>
    /// <returns>A view result that renders the "Login" view.</returns>
    public IActionResult Index()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            if (User.IsInRole("Admin"))
            
                return Redirect("/Admin/Index");
            
            
           
               return Redirect("/User/Index");
           
        }
        return View("Login");
    }

    public IActionResult Register()
    {
        return View("Register");
    }

}