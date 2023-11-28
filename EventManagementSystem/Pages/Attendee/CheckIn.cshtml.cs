using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Attendee
{
    public class CheckInModel : PageModel
    {
        [BindProperty]
        public Event EventToCheckIn { get; set; }

        public CheckInModel()
        {
            EventToCheckIn = new Event();
        }

        public IActionResult OnGet(int eventid)
        {
            if (HttpContext.Session.GetString("RoleType") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            HttpContext.Session.SetInt32("checkinevent", eventid);

            SqlDataReader singleEvent = DBClass.SingleEventReader(eventid);

            while (singleEvent.Read())
            {
                EventToCheckIn.EventID = eventid;
                EventToCheckIn.EventName = singleEvent["EventName"].ToString();
            }

            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery = "INSERT INTO EventCheckIn (EventID, UserID, CheckInDateTime) VALUES (" + HttpContext.Session.GetInt32("checkinevent") + ", " + HttpContext.Session.GetString("userid") + ", GETDATE())";
            DBClass.GeneralQuery(sqlQuery);
            DBClass.DBConnection.Close();

            return RedirectToPage("/Attendee/Index");
        }
    }
}
