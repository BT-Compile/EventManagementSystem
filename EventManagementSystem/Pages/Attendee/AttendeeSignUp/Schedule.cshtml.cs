using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Attendee.AttendeeSignUp
{
    public class ScheduleModel : PageModel
    {
        public List<Event> Events { get; set; }

        [BindProperty]
        public Event ParentEvent { get; set; }

        public ScheduleModel()
        {
            Events = new List<Event>();
            ParentEvent = new Event();
        }

        public IActionResult OnGet(int eventid)
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            HttpContext.Session.SetInt32("parentid", eventid);

            // query to select all SUBEVENTS within the EVENT selected on the previous page
            string sqlQuery = "SELECT Event.EventID, Event.EventName, Event.EventDescription, Event.StartDate, Event.EndDate, Event.RegistrationDeadline, Space.Name " +
                                "FROM  Event INNER JOIN EventSpace ON Event.EventID = EventSpace.EventID INNER JOIN Space ON EventSpace.SpaceID = Space.SpaceID " +
                                "WHERE EXISTS( " +
                                "SELECT " + HttpContext.Session.GetString("userid") +
                                "FROM EventRegister " +
                                "WHERE UserID = " + HttpContext.Session.GetString("userid") + " AND EventRegister.EventID = Event.EventID) " +
                                "AND ParentEventID = " + eventid + " ORDER BY Event.StartDate DESC";

            SqlDataReader scheduleReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (scheduleReader.Read())
            {
                Events.Add(new Event
                {
                    EventID = Int32.Parse(scheduleReader["EventID"].ToString()),
                    EventName = scheduleReader["EventName"].ToString(),
                    StartDate = (DateTime)scheduleReader["StartDate"],
                    EndDate = (DateTime)scheduleReader["EndDate"],
                    RegistrationDeadline = (DateTime)scheduleReader["RegistrationDeadline"],
                    SpaceID = scheduleReader["Name"].ToString()
                });
            }
            DBClass.DBConnection.Close();

            sqlQuery = "SELECT * FROM Event WHERE EventID = " + eventid;
            SqlDataReader singleActivity = DBClass.GeneralReaderQuery(sqlQuery);

            while (singleActivity.Read())
            {
                ParentEvent.EventID = eventid;
                ParentEvent.EventName = singleActivity["EventName"].ToString();
            }

            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPostRegister()
        {
            int? eventid = HttpContext.Session.GetInt32("parentid");
            return RedirectToPage("/Attendee/AttendeeSignUp/ActivitySignUpMore", new { eventid });
        }
    }
}
