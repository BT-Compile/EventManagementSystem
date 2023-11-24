using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EventManagementSystem.Pages.Login
{
    public class LogOutModel : PageModel
    {
        public IActionResult OnGet()
        {
            HttpContext.Session.Remove("username");
            HttpContext.Session.Remove("password");
            HttpContext.Session.Remove("userid");
            HttpContext.Session.Remove("RoleType");

            return RedirectToPage("Index");
        }
    }
}
