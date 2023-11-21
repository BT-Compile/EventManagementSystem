using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Admin
{
    public class ApproveEventModel : PageModel
    {
        [BindProperty]
        public Event EventToApprove { get; set; }

        public ApproveEventModel()
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

            string sqlQuery = "SELECT * FROM PendingEvent WHERE PendingEventID = " + eventID;
            SqlDataReader singleEvent = DBClass.GeneralReaderQuery(sqlQuery);

            while (singleEvent.Read())
            {
                EventToApprove.EventID = eventID;
                EventToApprove.EventName = singleEvent["EventName"].ToString();
                EventToApprove.UserID = Int32.Parse(singleEvent["UserID"].ToString());
            }

            DBClass.DBConnection.Close();

            HttpContext.Session.SetString("EventName", EventToApprove.EventName);
            HttpContext.Session.SetInt32("UserID", EventToApprove.UserID);

            return Page();
        }

        public IActionResult OnPost()
        {
            //Inserts Event info from PendingEvent into Event table
            string sqlQuery = "INSERT INTO [Event] (EventName, EventDescription, StartDate, EndDate, RegistrationDeadline, Capacity, [Type]) " +
                            "SELECT EventName, EventDescription, StartDate, EndDate, RegistrationDeadline, Capacity, [Type] FROM PendingEvent " +
                            "WHERE PendingEvent.PendingEventID = " + EventToApprove.EventID;

            DBClass.GeneralQuery(sqlQuery);

            DBClass.DBConnection.Close();

            //Sets Status of Event to Active
            sqlQuery = "UPDATE [Event] SET [Status] = 'Active' " +
               "WHERE EventName = '" + HttpContext.Session.GetString("EventName") + "'";

            DBClass.GeneralQuery(sqlQuery);

            DBClass.DBConnection.Close();

            //Creates a bridge between Organizer and Event that they created
            sqlQuery = "INSERT INTO EventCreate (EventID, UserID, CreationDate) " +
               "SELECT PendingEventID, UserID, StartDate FROM PendingEvent WHERE PendingEvent.EventName = '" + HttpContext.Session.GetString("EventName") + "'";

            DBClass.GeneralQuery(sqlQuery);

            DBClass.DBConnection.Close();

            //Sets a variable that we can use for EventID for the next query
            sqlQuery = "SELECT * FROM Event WHERE EventName = '" + HttpContext.Session.GetString("EventName") + "'";
            SqlDataReader singleEvent = DBClass.GeneralReaderQuery(sqlQuery);

            while (singleEvent.Read())
            {
                EventToApprove.EventID = Int32.Parse(singleEvent["EventID"].ToString());
            }

            DBClass.DBConnection.Close();

            //Updates info in EventCreate to match what is on record
            sqlQuery = "UPDATE EventCreate SET EventID = " + EventToApprove.EventID + ", CreationDate = GETDATE() " +
               "WHERE UserID = " + HttpContext.Session.GetInt32("UserID");

            DBClass.GeneralQuery(sqlQuery);

            DBClass.DBConnection.Close();

            //Deletes Pending event now that it is approved.
            sqlQuery = "DELETE FROM PendingEvent WHERE EventName = '" + HttpContext.Session.GetString("EventName") + "'";

            DBClass.GeneralQuery(sqlQuery);

            DBClass.DBConnection.Close();

            return RedirectToPage("/Admin/ApproveDeclineEvent");
        }
    }
}
