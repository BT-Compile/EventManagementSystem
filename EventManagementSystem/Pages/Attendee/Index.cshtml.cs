using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Attendee
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string FullName { get; set; }

        [BindProperty]
        public string TeamName { get; set; }

        public List<Event> Events { get; set; }

        public IndexModel()
        {
            Events = new List<Event>();
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

            // query to select all events that this user has signed up for already
            string sqlQuery = "SELECT [Event].EventID, [Event].EventName, [Event].EventDescription, [Event].StartDate, [Event].EndDate, [Event].RegistrationDeadline, Building.Name AS BuildingName " +
                "FROM [User] " +
                "INNER JOIN EventAttendance ON[User].UserID = EventAttendance.UserID " +
                "INNER JOIN[Event] ON EventAttendance.EventID = [Event].EventID " +
                "INNER JOIN Building ON[Event].BuildingID = Building.BuildingID " +
                "WHERE [User].UserID = " + HttpContext.Session.GetString("userid") +
                " ORDER BY [Event].StartDate DESC";

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
                    BuildingName = scheduleReader["BuildingName"].ToString()
                });
            }

            return Page();
        }
    }
}
