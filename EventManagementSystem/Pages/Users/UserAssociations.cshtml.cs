using EventManagementSystem.Pages.DB;
using EventManagementSystem.Pages.Rooms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Users
{
    public class UserAssociationsModel : PageModel
    {
        [BindProperty]
        public int UserID { get; set; }

        public List<SelectListItem> Users { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("RoleType") != "Admin" &&
                (HttpContext.Session.GetString("RoleType") == "Presenter" || HttpContext.Session.GetString("RoleType") == "Judge"
                || HttpContext.Session.GetString("RoleType") == "Participant" || HttpContext.Session.GetString("RoleType") == "Organizer"))
            {
                return RedirectToPage("/Attendee/Index");
            }
            else if (HttpContext.Session.GetString("RoleType") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            // Populate the Event SELECT control
            SqlDataReader UserReader = DBClass.GeneralReaderQuery("SELECT * FROM \"User\"");

            Users = new List<SelectListItem>();

            while (UserReader.Read())
            {
                Users.Add(new SelectListItem
                (
                    UserReader["UserName"].ToString(),
                    UserReader["UserID"].ToString()
                ));
            }

            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            if (UserID != 0)
            {
                // Redirect to the Schedule page with the selected EventID
                return RedirectToPage("UserSchedule", new { userid = UserID });
            }
            else
            {
                // Handle the case where no event is selected
                return Page();
            }
        }
    }
}
