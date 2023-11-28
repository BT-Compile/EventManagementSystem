using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Organizer
{
    public class RemoveParticipantEventModel : PageModel
    {
        [BindProperty]
        public User UserToCancel { get; set; }

        [BindProperty]
        public Event EventToCancel { get; set; }

        public RemoveParticipantEventModel()
        {
            UserToCancel = new User();
            EventToCancel = new Event();
        }

        public IActionResult OnGet(int userid, int eventid)
        {
            if (HttpContext.Session.GetString("RoleType") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            HttpContext.Session.SetInt32("routeuserid", userid);
            HttpContext.Session.SetInt32("routeeventid", eventid);

            string sqlQuery = "SELECT * FROM [User] " +
                "WHERE UserID = '" + userid + "';";
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
            string sqlQuery = "DELETE FROM EventRegister WHERE UserID = " + HttpContext.Session.GetInt32("routeuserid") + " AND EventID IN (Select Event.EventID FROM  Event INNER JOIN " +
                                "EventRegister ON Event.EventID = EventRegister.EventID WHERE ParentEventID = " + HttpContext.Session.GetInt32("routeeventid") + ")";
            DBClass.GeneralQuery(sqlQuery);
            DBClass.DBConnection.Close();

            sqlQuery = "DELETE FROM EventRegister " +
                "WHERE UserID = " + HttpContext.Session.GetInt32("routeuserid") +
                " AND EventID = " + HttpContext.Session.GetInt32("routeeventid");
            DBClass.GeneralQuery(sqlQuery);
            DBClass.DBConnection.Close();

            int? eventid = HttpContext.Session.GetInt32("routeeventid");

            return RedirectToPage("/Organizer/ParticipantList", new { eventid });
        }
    }
}
