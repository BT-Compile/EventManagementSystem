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
            if (HttpContext.Session.GetString("usertype") != "Admin" && HttpContext.Session.GetString("usertype") == "Attendee")
            {
                return RedirectToPage("/Attendee/Index");
            }
            else if (HttpContext.Session.GetString("usertype") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery = "INSERT INTO Activity (ActivityName, ActivityDescription, DateAndTime, IsPresentation, IsMeeting, IsProgramEvent, EventID, IsActive) VALUES (" +
                "'" + ActivityToCreate.ActivityName + "'," +
                "'" + ActivityToCreate.ActivityDescription + "'," +
                "'" + ActivityToCreate.DateAndTime.ToString("yyyy-MM-dd HH:mm:ss") + "'," +
                "'" + ActivityToCreate.IsPresentation + "'," +
                "'" + ActivityToCreate.IsMeeting + "'," +
                "'" + ActivityToCreate.IsProgramEvent + "',"
                + ActivityToCreate.EventID + ",1)";

            DBClass.GeneralQuery(sqlQuery);

            DBClass.DBConnection.Close();

            return RedirectToPage("AdminActivity");
        }

    }
}
