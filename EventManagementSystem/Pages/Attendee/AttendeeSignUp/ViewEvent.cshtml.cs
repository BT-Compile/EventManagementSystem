using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Attendee.AttendeeSignUp
{
    public class ViewEventModel : PageModel
    {
        public List<Event> Events { get; set; }

        public List<Event> Subevents { get; set; }

        [BindProperty]
        public Event RegisterCount { get; set; }

        public ViewEventModel()
        {
            Events = new List<Event>();
            Subevents = new List<Event>();
            RegisterCount = new Event();
        }
        public IActionResult OnGet(int eventid)
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            // query to select the event selected and display all event details in detail
            string sqlQuery = "SELECT Event.*, [Space].*, Location.* FROM Location INNER JOIN " +
                               "[Space] ON Location.LocationID = [Space].LocationID INNER JOIN Event INNER JOIN EventSpace ON Event.EventID = EventSpace.EventID ON[Space].SpaceID = EventSpace.SpaceID " +
                               "WHERE Event.EventID = " + eventid +
                               " ORDER BY Event.StartDate DESC";

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
                    Capacity = Int32.Parse(scheduleReader["Capacity"].ToString()),
                    EventType = scheduleReader["Type"].ToString(),
                    Status = scheduleReader["Status"].ToString(),
                    SpaceName = scheduleReader["Name"].ToString(),
                    SpaceAddress = scheduleReader["Address"].ToString()
                });
            }

            DBClass.DBConnection.Close();

            sqlQuery = "SELECT Event.*, Space.*, Location.*, CASE WHEN Space.ParentSpaceID IS NOT NULL THEN ParentSpace.Name ELSE NULL " +
                       "END AS ParentSpaceName FROM Event INNER JOIN EventSpace ON Event.EventID = EventSpace.EventID INNER JOIN Space ON EventSpace.SpaceID = Space.SpaceID INNER JOIN " +
                       "Location ON Space.LocationID = Location.LocationID LEFT JOIN Space ParentSpace ON Space.ParentSpaceID = ParentSpace.SpaceID " +
                       "WHERE [Event].ParentEventID = " + eventid +
                       " ORDER BY Event.StartDate ASC";

            SqlDataReader subeventReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (subeventReader.Read())
            {
                Subevents.Add(new Event
                {
                    EventID = Int32.Parse(subeventReader["EventID"].ToString()),
                    EventName = subeventReader["EventName"].ToString(),
                    EventDescription = subeventReader["EventDescription"].ToString(),
                    StartDate = (DateTime)subeventReader["StartDate"],
                    EndDate = (DateTime)subeventReader["EndDate"],
                    RegistrationDeadline = (DateTime)subeventReader["RegistrationDeadline"],
                    Capacity = Int32.Parse(subeventReader["Capacity"].ToString()),
                    EventType = subeventReader["Type"].ToString(),
                    Status = subeventReader["Status"].ToString(),
                    SpaceName = subeventReader["Name"].ToString(),
                    SpaceAddress = subeventReader["Address"].ToString(),
                    ParentSpaceName = subeventReader["ParentSpaceName"].ToString()
                });
            }

            DBClass.DBConnection.Close();

            sqlQuery = "SELECT COUNT(EventRegister.UserID) AS RegisterCount FROM Event INNER JOIN " +
                                         "EventRegister ON Event.EventID = EventRegister.EventID INNER JOIN [User] ON EventRegister.UserID = [User].UserID INNER JOIN " +
                                         "UserRole ON[User].UserID = UserRole.UserID INNER JOIN Role ON UserRole.RoleID = Role.RoleID " +
                                         "WHERE Event.EventID = " + eventid;

            SqlDataReader registercCount = DBClass.GeneralReaderQuery(sqlQuery);

            while (registercCount.Read())
            {
                RegisterCount.Capacity = Int32.Parse(registercCount["RegisterCount"].ToString());
            }

            DBClass.DBConnection.Close();
            return Page();
        }
    }
}
