using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EventManagementSystem.Pages.Login
{
    public class LogOutModel : PageModel
    {
        public IActionResult OnGet()
        {
            //if (HttpContext.Session.GetString("username") == null)
            //{
            //    return RedirectToPage("/Login/Index");
            //}

            HttpContext.Session.Remove("username");
            HttpContext.Session.Remove("password");
            HttpContext.Session.Remove("userid");
            HttpContext.Session.Remove("RoleType");

            return Page();
        }
    }
}
