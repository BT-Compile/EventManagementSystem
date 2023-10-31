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

            string nameQuery = "SELECT FirstName, LastName FROM \"User\" WHERE Username = '" + HttpContext.Session.GetString("username") + "'";

            SqlDataReader nameReader = DBClass.GeneralReaderQuery(nameQuery);

            if (nameReader.Read())
            {
                FullName = nameReader["FirstName"].ToString() + " " + nameReader["LastName"].ToString();
            }

            nameReader.Close();

            string sqlQuery = "SELECT Activity.ActivityID, Event.EventName, Event.EventDescription, " +
                "Event.EventLocation, Activity.ActivityName, Activity.ActivityDescription, " +
                "Activity.DateAndTime, Room.RoomName " +
                "FROM \"User\" " +
                "INNER JOIN Attendance ON \"User\".UserID = Attendance.UserID " +
                "INNER JOIN Activity ON Attendance.ActivityID = Activity.ActivityID " +
                "INNER JOIN Event ON Activity.EventID = Event.EventID " +
                "LEFT JOIN Room ON Activity.RoomID = Room.RoomID " +
                "WHERE \"User\".Username = '" + HttpContext.Session.GetString("username") + "' " +
                "ORDER BY Activity.DateAndTime;";

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
                }) ;
            }

            scheduleReader.Close();
            DBClass.LabDBConnection.Close();

            return Page();
        }
    }
}
