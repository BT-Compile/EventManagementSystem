using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Attendee.AttendeeSignUp
{
    public class ActivitySignUpModel : PageModel
    {
        public List<Event> Events { get; set; }

        [BindProperty]
        public List<int> Checked { get; set; }

        [BindProperty]
        public Event ParentEvent { get; set; }

        public static int ParentEventStatic {  get; set; }

        public ActivitySignUpModel()
        {
            Events = new List<Event>();
            ParentEvent = new Event();
        }

        public IActionResult OnGet(int eventID)
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            // query to select all SUBEVENTS within the EVENT selected on the previous page
            string sqlQuery = "SELECT Event.EventID, Event.EventName, Event.EventDescription, Event.StartDate, Event.EndDate, Event.RegistrationDeadline, Space.Name " +
                                "FROM  Event INNER JOIN EventSpace ON Event.EventID = EventSpace.EventID INNER JOIN Space ON EventSpace.SpaceID = Space.SpaceID " +
                                "WHERE NOT EXISTS( " +
                                "SELECT " + HttpContext.Session.GetString("userid") +
                                "FROM EventRegister " +
                                "WHERE UserID = " + HttpContext.Session.GetString("userid") + " AND EventRegister.EventID = Event.EventID) " +
                                "AND ParentEventID = " + eventID + " ORDER BY Event.StartDate DESC";

            SqlDataReader scheduleReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (scheduleReader.Read())
            {
                Events.Add(new Event
                {
                    EventID = Int32.Parse(scheduleReader["EventID"].ToString()),
                    EventName = scheduleReader["EventName"].ToString(),
                    EventDescription = scheduleReader["EventDescription"].ToString(),
                    StartDate = (DateTime)scheduleReader["StartDate"],
                    EndDate = (DateTime)scheduleReader["EndDate"],
                    RegistrationDeadline = (DateTime)scheduleReader["RegistrationDeadline"],
                    SpaceName = scheduleReader["Name"].ToString()
                });
            }
            DBClass.DBConnection.Close();

            sqlQuery = "SELECT * FROM Event WHERE EventID = " + eventID;
            SqlDataReader singleActivity = DBClass.GeneralReaderQuery(sqlQuery);

            while (singleActivity.Read())
            {
                ParentEvent.EventID = eventID;
                ParentEvent.EventName = singleActivity["EventName"].ToString();
            }

            DBClass.DBConnection.Close();

            return Page();
        }
        public IActionResult OnPost(List<int> Checked)
        {
            string sqlQuery = "SELECT * FROM Event WHERE EventID = " + ParentEvent.EventID;
            SqlDataReader singleActivity = DBClass.GeneralReaderQuery(sqlQuery);

            while (singleActivity.Read())
            {
                ParentEvent.EventID = Int32.Parse(singleActivity["ParentEventID"].ToString());
            }

            DBClass.DBConnection.Close();

            sqlQuery = "INSERT INTO EventRegister (EventID, UserID, RegistrationDate) VALUES (" +
                ParentEvent.EventID + ", " + HttpContext.Session.GetString("userid") + ", GETDATE())";
            DBClass.GeneralQuery(sqlQuery);

            DBClass.DBConnection.Close();

            foreach (int i in Checked)
            {
                sqlQuery = "INSERT INTO EventRegister (EventID, UserID, RegistrationDate) VALUES (" +
                i + ", " + HttpContext.Session.GetString("userid") + ", GETDATE())";
                DBClass.GeneralQuery(sqlQuery);
                DBClass.DBConnection.Close();
            }

            //DBClass.DBConnection.Close();

            return RedirectToPage("../Index");
        }
    }
}
