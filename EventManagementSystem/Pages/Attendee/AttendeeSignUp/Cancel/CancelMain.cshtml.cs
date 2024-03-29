using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Attendee.AttendeeSignUp.Cancel
{
    public class CancelMainModel : PageModel //Cancels registration for single SUBEVENTS
    {
        [BindProperty]
        public User UserToCancel { get; set; }

        [BindProperty]
        public Event EventToCancel { get; set; }

        public int? TempParent {  get; set; }

        public CancelMainModel()
        {
            UserToCancel = new User();
            EventToCancel = new Event();
        }

        public IActionResult OnGet(int eventid)
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            string sqlQuery;

            sqlQuery = "SELECT * FROM Event WHERE EventID = " + eventid;
            SqlDataReader singlePEvent = DBClass.GeneralReaderQuery(sqlQuery);

            while (singlePEvent.Read())
            {
                EventToCancel.TempParentID = Int32.Parse(singlePEvent["ParentEventID"].ToString());
            }

            HttpContext.Session.SetInt32("parentid", EventToCancel.TempParentID);

            TempParent = HttpContext.Session.GetInt32("parentid");

            DBClass.DBConnection.Close();

            sqlQuery = "SELECT * FROM \"User\" " +
                "WHERE Username = '" + HttpContext.Session.GetString("username") + "';";
            SqlDataReader singleUser = DBClass.GeneralReaderQuery(sqlQuery);

            while (singleUser.Read())
            {
                UserToCancel.UserID = Int32.Parse(singleUser["UserID"].ToString());
                UserToCancel.FirstName = singleUser["FirstName"].ToString();
                UserToCancel.LastName = singleUser["LastName"].ToString();
            }

            DBClass.DBConnection.Close();

            sqlQuery = "SELECT * FROM Event WHERE EventID = " + eventid;
            SqlDataReader singleEvent = DBClass.GeneralReaderQuery(sqlQuery);

            while (singleEvent.Read())
            {
                EventToCancel.EventID = eventid;
                EventToCancel.EventName = singleEvent["EventName"].ToString();
            }

            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            string ssqlQuery = "DELETE FROM EventRegister " +
                "WHERE UserID = " + HttpContext.Session.GetString("userid") +
                " AND EventID = " + EventToCancel.EventID;
            DBClass.GeneralQuery(ssqlQuery);
            DBClass.DBConnection.Close();

            int? eventid = HttpContext.Session.GetInt32("parentid");

            return RedirectToPage("/Attendee/AttendeeSignUp/Schedule", new { eventid });
        }
    }
}
