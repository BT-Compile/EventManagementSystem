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
            if (HttpContext.Session.GetString("usertype") != "Admin" && HttpContext.Session.GetString("usertype") == "Attendee")
            {
                return RedirectToPage("/Attendee/Index");
            }
            else if (HttpContext.Session.GetString("usertype") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            SqlDataReader singleActivity = DBClass.SingleActivityReader(activityid);

            while (singleActivity.Read())
            {
                ActivityToDelete.ActivityID = activityid;
            }

            DBClass.LabDBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery = "UPDATE Activity SET IsActive = 0, EventID = null WHERE ActivityID = " + ActivityToDelete.ActivityID;

            DBClass.GeneralQuery(sqlQuery);

            DBClass.LabDBConnection.Close();

            return RedirectToPage("/Activities/AdminActivity");
        }
    }
}
