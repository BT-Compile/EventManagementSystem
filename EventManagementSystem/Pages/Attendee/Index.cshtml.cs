using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Attendee
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string FullName { get; set; }

        public List<AttendeeSchedule> attendeeSchedules { get; set; }

        public IndexModel()
        {
            attendeeSchedules = new List<AttendeeSchedule>();
        }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            // Retrive the full name of this logged in User's name
            string nameQuery = "SELECT FirstName, LastName FROM \"User\" WHERE Username = '" + HttpContext.Session.GetString("username") + "'";
            SqlDataReader nameReader = DBClass.GeneralReaderQuery(nameQuery);
            if (nameReader.Read())
            {
                FullName = nameReader["FirstName"].ToString() + " " + nameReader["LastName"].ToString();
            }
            nameReader.Close();

            // Query to select all user's registered events based on username
            string sqlQuery = "SELECT Activity.ActivityID, Event.EventName, Event.EventDescription, Activity.ActivityName, " +
                                "Activity.ActivityDescription, Activity.[Date], Activity.StartTime, Building.[Name], Room.RoomNumber " +
                                "FROM  Activity INNER JOIN ActivityAttendance ON Activity.ActivityID = ActivityAttendance.ActivityID " +
                                "INNER JOIN Event ON Activity.EventID = Event.EventID " +
                                "INNER JOIN Building ON Event.BuildingID = Building.BuildingID " +
                                "INNER JOIN EventAttendance ON Event.EventID = EventAttendance.EventID " +
                                "INNER JOIN Room ON Activity.RoomID = Room.RoomID AND Building.BuildingID = Room.BuildingID " +
                                "INNER JOIN [User] ON ActivityAttendance.UserID = [User].UserID AND EventAttendance.UserID = [User].UserID " +
                                "WHERE [User].Username = '" + HttpContext.Session.GetString("username") + "' " +
                                "ORDER BY Activity.[Date];";

            SqlDataReader scheduleReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (scheduleReader.Read())
            {
                attendeeSchedules.Add(new AttendeeSchedule
                {
                    ActivityID = Int32.Parse(scheduleReader["ActivityID"].ToString()),
                    EventName = scheduleReader["EventName"].ToString(),
                    EventDescription = scheduleReader["EventDescription"].ToString(),
                    ActivityName = scheduleReader["ActivityName"].ToString(),
                    ActivityDescription = scheduleReader["ActivityDescription"].ToString(),
                    Date = DateTime.Parse(scheduleReader["Date"].ToString()),
                    StartTime = TimeOnly.Parse(scheduleReader["StartTime"].ToString()),
                    BuildingName = scheduleReader["BuildingName"].ToString(),
                    RoomNumber = Int32.Parse(scheduleReader["RoomNumber"].ToString())
                }) ;
            }

            scheduleReader.Close();
            DBClass.DBConnection.Close();

            return Page();
        }
    }
}
