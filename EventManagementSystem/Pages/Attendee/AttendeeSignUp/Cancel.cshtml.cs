using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Attendee.AttendeeSignUp
{
    public class CancelModel : PageModel //Cancels registration for overarching EVENTS
    {
        [BindProperty]
        public User UserToCancel { get; set; }

        [BindProperty]
        public Event EventToCancel { get; set; }

        public CancelModel()
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
            string sqlQuery = "DELETE FROM EventRegister WHERE UserID = " + HttpContext.Session.GetString("userid") + " AND EventID IN (Select Event.EventID FROM  Event INNER JOIN " +
                                "EventRegister ON Event.EventID = EventRegister.EventID WHERE ParentEventID = " + EventToCancel.EventID + ")";
            DBClass.GeneralQuery(sqlQuery);
            DBClass.DBConnection.Close();

            
            sqlQuery = "DELETE FROM EventRegister " +
                "WHERE UserID = " + HttpContext.Session.GetString("userid") +
                " AND EventID = " + EventToCancel.EventID;
            DBClass.GeneralQuery(sqlQuery);
            DBClass.DBConnection.Close();

            sqlQuery = "DELETE FROM EventCheckIn " +
                "WHERE UserID = " + HttpContext.Session.GetString("userid") +
                " AND EventID = " + EventToCancel.EventID;
            DBClass.GeneralQuery(sqlQuery);
            DBClass.DBConnection.Close();

            return RedirectToPage("../Index");
        }
    }
}