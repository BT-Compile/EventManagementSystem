using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Admin
{
    public class DeclineEventModel : PageModel
    {
        [BindProperty]
        public Event EventToApprove { get; set; }

        public DeclineEventModel()
        {
            EventToApprove = new Event();
        }
        public IActionResult OnGet(int eventID)
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

            string sqlQuery = "SELECT * FROM Event WHERE EventID = " + eventID;
            SqlDataReader singleEvent = DBClass.GeneralReaderQuery(sqlQuery);

            while (singleEvent.Read())
            {
                EventToApprove.EventID = eventID;
                EventToApprove.EventName = singleEvent["EventName"].ToString();
                EventToApprove.UserID = Int32.Parse(singleEvent["OrganizerID"].ToString());
            }

            DBClass.DBConnection.Close();

            HttpContext.Session.SetString("EventName", EventToApprove.EventName);
            HttpContext.Session.SetInt32("OrganizerID", EventToApprove.UserID);
            HttpContext.Session.SetInt32("EventID", EventToApprove.EventID);

            return Page();
        }

        public IActionResult OnPost()
        {
            //Sets Status of Event to Not Approved
            string sqlQuery = "UPDATE [Event] SET [Status] = 'Not Approved' " +
               "WHERE EventID = " + HttpContext.Session.GetInt32("EventID");

            DBClass.GeneralQuery(sqlQuery);

            DBClass.DBConnection.Close();

            return RedirectToPage("/Admin/ApproveDeclineEvent");
        }
    }
}
