using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Attendee
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string FullName { get; set; }

        [BindProperty]
        public string TeamName { get; set; }

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
            string scheduleQuery = "SELECT [Event].EventName, [Event].EventDescription, [Event].StartDate, [Event].EndDate, [Activity].ActivityName, [Activity].ActivityDescription, [Activity].Date AS ActivityDate, [Activity].StartTime AS ActivityStartTime, [Activity].EndTime AS ActivityEndTime, [Room].RoomNumber, Building.Name AS BuildingName " +
                "FROM[User] " +
                "INNER JOIN EventAttendance ON[User].UserID = EventAttendance.UserID " +
                "INNER JOIN[Event] ON EventAttendance.EventID = [Event].EventID " +
                "LEFT JOIN ActivityAttendance ON[User].UserID = ActivityAttendance.UserID " +
                "LEFT JOIN Activity ON ActivityAttendance.ActivityID = Activity.ActivityID " +
                "LEFT JOIN Room ON Activity.RoomID = Room.RoomID " +
                "LEFT JOIN Building ON Room.BuildingID = Building.BuildingID " +
                "WHERE[User].Username = '" + HttpContext.Session.GetString("username") + "' " +
                "ORDER BY[Activity].EndTime DESC, [Activity].Date DESC";
            SqlDataReader scheduleReader = DBClass.GeneralReaderQuery(scheduleQuery);

            while (scheduleReader.Read())
            {
                attendeeSchedules.Add(new AttendeeSchedule
                {
                    EventName = scheduleReader["EventName"].ToString(),
                    EventDescription = scheduleReader["EventDescription"].ToString(),
                    StartDate = DateOnly.Parse(scheduleReader["StartDate"].ToString()),
                    EndDate = DateOnly.Parse(scheduleReader["EndDate"].ToString()),
                    ActivityName = scheduleReader["ActivityName"].ToString(),
                    ActivityDescription = scheduleReader["ActivityDescription"].ToString(),
                    ActivityDate = DateOnly.Parse(scheduleReader["ActivityDate"].ToString()),
                    StartTime = TimeOnly.Parse(scheduleReader["StartTime"].ToString()),
                    EndTime = TimeOnly.Parse(scheduleReader["EndTime"].ToString()),
                    BuildingName = scheduleReader["BuildingName"].ToString(),
                    RoomNumber = Int32.Parse(scheduleReader["RoomNumber"].ToString())
                }) ;
            }
            scheduleReader.Close();
            DBClass.DBConnection.Close();

            // Retrieve this participant's team name (if any)
            string teamNameQuery = "SELECT Team.Name AS TeamName " +
                "FROM [User] " +
                "INNER JOIN UserTeam ON [User].UserID = UserTeam.UserID " +
                "INNER JOIN Team ON UserTeam.TeamID = Team.TeamID " +
                "WHERE [User].Username = '" + HttpContext.Session.GetString("username") + "'";
            SqlDataReader teamNameReader = DBClass.GeneralReaderQuery(teamNameQuery);

            if (teamNameReader.Read())
            {
                TeamName = teamNameReader["TeamName"].ToString();
            }
            teamNameReader.Close();

            return Page();
        }
    }
}
