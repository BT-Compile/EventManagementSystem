using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Users
{
    public class ActivateUserModel : PageModel
    {
        [BindProperty]
        public User UserToAdd { get; set; }

        public ActivateUserModel()
        {
            UserToAdd = new User();
        }

        public IActionResult OnGet(int userid)
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

            HttpContext.Session.SetInt32("tempuserid", userid);

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery = "UPDATE [User] SET IsActive = 1 WHERE UserID = " + HttpContext.Session.GetInt32("tempuserid");

            DBClass.GeneralQuery(sqlQuery);

            DBClass.DBConnection.Close();

            return RedirectToPage("Index");
        }
    }
}
