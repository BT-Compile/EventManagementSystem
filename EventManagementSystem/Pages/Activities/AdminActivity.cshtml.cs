using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Activities
{
    public class AdminActivityModel : PageModel
    {
        public List<Activity> Activities { get; set; }

        public AdminActivityModel()
        {
            Activities = new List<Activity>();
        }

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

            string sqlQuery = "SELECT * FROM Activity";
            SqlDataReader activityReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (activityReader.Read())
            {
                // Check if EventID is NULL
                int? eventID = activityReader["EventID"] is DBNull ? (int?)null : (int)activityReader["EventID"];

                Activities.Add(new Activity
                {
                    ActivityID = Int32.Parse(activityReader["ActivityID"].ToString()),
                    ActivityName = activityReader["ActivityName"].ToString(),
                    ActivityDescription = activityReader["ActivityDescription"].ToString(),
                    Date = (DateTime)activityReader["Date"],
                    IsPresentation = (bool)activityReader["IsPresentation"],
                    IsMeeting = (bool)activityReader["IsMeeting"],
                    IsProgramEvent = (bool)activityReader["IsProgramEvent"],
                    EventID = eventID,
                    IsActive = (bool)activityReader["IsActive"]
                });
            }

            DBClass.DBConnection.Close();

            return Page();
        }
    }
}
