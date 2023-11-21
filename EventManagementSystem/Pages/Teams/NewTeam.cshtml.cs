using EventManagementSystem.Pages.DataClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EventManagementSystem.Pages.Teams
{
    public class NewTeamModel : PageModel
    {
        [BindProperty]
        public Team TeamToCreate { get; set; }



        public void OnGet()
        {
        }
    }
}
