using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EventManagementSystem.Pages.Users
{
    public class SearchModel : PageModel
    {
        [BindProperty]
        public string User { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            return RedirectToPage("/Users/SearchResult", new { User });
        }

        public IActionResult OnPostReturnHandler()
        {
            return RedirectToPage("Index");
        }
    }
}
