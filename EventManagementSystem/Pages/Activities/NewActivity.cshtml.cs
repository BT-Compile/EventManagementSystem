using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EventManagementSystem.Pages.Activities
{
    public class NewActivityModel : PageModel
    {
        [BindProperty]
        public Activity ActivityToCreate { get; set; }

        public NewActivityModel()
        {
            ActivityToCreate = new Activity();
        }

        // No Activity is needed to get in this method,
        // we are not updating an Activity, only creating a new one
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

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery = "INSERT INTO Activity (ActivityName, ActivityDescription, Date, StartTime, EndTime, Type, [Status], EventID, RoomID) VALUES (" +
                "'" + ActivityToCreate.ActivityName + "'," +
                "'" + ActivityToCreate.ActivityDescription + "'," +
                "'" + ActivityToCreate.Date.ToString("yyyy-MM-dd") + "'," +
                "'" + ActivityToCreate.StartTime + "'," +
                "'" + ActivityToCreate.EndTime + "'," +
                "'" + ActivityToCreate.Type + "'," +
                "'" + ActivityToCreate.Status + "',"
                + ActivityToCreate.EventID + ","
                + ActivityToCreate.RoomID + ")";

            DBClass.GeneralQuery(sqlQuery);

            DBClass.DBConnection.Close();

            return RedirectToPage("AdminActivity");
        }

    }
}
