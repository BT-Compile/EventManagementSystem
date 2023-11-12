using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Attendee.AttendeeSignUp
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public int ActivityID { get; set; }

        [BindProperty]
        public string? Message { get; set; }

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

            // query to select all relevant information and also exclude all activities
            // this user has signed up for already
            string sqlQuery = "SELECT Activity.ActivityID, Event.EventName, Event.EventDescription, Activity.ActivityName, " +
                                "Activity.ActivityDescription, Activity.[Date], Activity.StartTime, Building.[Name], Room.RoomNumber " +
                                "FROM  Activity INNER JOIN ActivityAttendance ON Activity.ActivityID = ActivityAttendance.ActivityID " +
                                "INNER JOIN Event ON Activity.EventID = Event.EventID INNER JOIN " +
                                "Building ON Event.BuildingID = Building.BuildingID INNER JOIN " +
                                "EventAttendance ON Event.EventID = EventAttendance.EventID INNER JOIN " +
                                "Room ON Activity.RoomID = Room.RoomID AND Building.BuildingID = Room.BuildingID INNER JOIN " +
                                "[User] ON ActivityAttendance.UserID = [User].UserID AND EventAttendance.UserID = [User].UserID " +
                                "WHERE Activity.ActivityID NOT IN (" +
                                    "SELECT ActivityID " +
                                    "FROM ActivityAttendance " +
                                    "WHERE UserID = " + HttpContext.Session.GetString("userid") + ") " +
                                "ORDER BY Event.EventName, Activity.Date;";

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
                    
                    StartTime = TimeOnly.Parse(scheduleReader["StartTime"].ToString()),
                    BuildingName = scheduleReader["Name"].ToString(),
                    RoomNumber = Int32.Parse(scheduleReader["RoomNumber"].ToString())
                });
            }

            DBClass.DBConnection.Close();

            return Page();
        }
    }
}
