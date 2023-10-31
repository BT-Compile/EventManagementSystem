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
            string sqlQuery = "SELECT Event.EventName, Event.EventDescription, " +
                "Activity.ActivityName, Activity.DateAndTime, " +
                "Activity.ActivityDescription, Event.EventLocation, " +
                "Room.RoomName, Activity.ActivityID " +
                "FROM Event " +
                "INNER JOIN Activity ON Event.EventID = Activity.EventID " +
                "LEFT JOIN Room ON Activity.RoomID = Room.RoomID " +
                "WHERE Activity.ActivityID NOT IN (" +
                    "SELECT ActivityID " +
                    "FROM Attendance " +
                    "WHERE UserID = " + HttpContext.Session.GetString("userid") + ") " +
                "ORDER BY Event.EventName, Activity.DateAndTime;";
            SqlDataReader scheduleReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (scheduleReader.Read())
            {
                attendeeSchedules.Add(new AttendeeSchedule
                {
                    ActivityID = Int32.Parse(scheduleReader["ActivityID"].ToString()),
                    EventName = scheduleReader["EventName"].ToString(),
                    EventDescription = scheduleReader["EventDescription"].ToString(),
                    EventLocation = scheduleReader["EventLocation"].ToString(),
                    ActivityName = scheduleReader["ActivityName"].ToString(),
                    ActivityDescription = scheduleReader["ActivityDescription"].ToString(),
                    DateAndTime = (DateTime)scheduleReader["DateAndTime"],
                    RoomName = scheduleReader["RoomName"].ToString()
                });
            }

            DBClass.LabDBConnection.Close();

            return Page();
        }
    }
}
