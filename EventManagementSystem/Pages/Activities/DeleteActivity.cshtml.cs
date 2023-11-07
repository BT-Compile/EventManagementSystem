using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Activities
{
    public class DeleteActivityModel : PageModel
    {
        [BindProperty]
        public Activity ActivityToDelete { get; set; }

        public DeleteActivityModel()
        {
            ActivityToDelete = new Activity();
        }

        public IActionResult OnGet(int activityid)
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

            SqlDataReader singleActivity = DBClass.SingleActivityReader(activityid);

            while (singleActivity.Read())
            {
                ActivityToDelete.ActivityID = activityid;
            }

            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery = "UPDATE Activity SET IsActive = 0, EventID = null WHERE ActivityID = " + ActivityToDelete.ActivityID;

            DBClass.GeneralQuery(sqlQuery);

            DBClass.DBConnection.Close();

            return RedirectToPage("/Activities/AdminActivity");
        }
    }
}
