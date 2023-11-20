using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Attendee
{
    public class TeamSignUpModel : PageModel
    {
        public List<Team> Teams { get; set; }

        [BindProperty]
        public bool HasTeam { get; set; }

        public TeamSignUpModel()
        {
            Teams = new List<Team>();
            HasTeam = false;
        }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            // Retrieve all of the available team names that the user is not signed up for.
            string teamNamesQuery = "SELECT Team.TeamID, Team.Name, Team.Description, Team.MaxSize " +
                "FROM Team " +
                "WHERE Team.TeamID NOT IN (" +
                "   SELECT UserTeam.TeamID " +
                "   FROM[User] " +
                "   INNER JOIN UserTeam ON[User].UserID = UserTeam.UserID " +
                "   WHERE[User].Username = '" + HttpContext.Session.GetString("username") + "')";

            SqlDataReader teamNamesReader = DBClass.GeneralReaderQuery(teamNamesQuery);
            while (teamNamesReader.Read())
            {
                Teams.Add(new Team
                {
                    TeamID = Int32.Parse(teamNamesReader["TeamID"].ToString()),
                    Name = teamNamesReader["Name"].ToString(),
                    Description = teamNamesReader["Description"].ToString()
                });
            }

            // Retrieve the TeamID that the user is currently on (if any) and binds it to a true value,
            // signifying that the user logged in already is on a team.
            // This is used as a failsafe against leaving a team without already being on one
            string teamNameQuery = "SELECT TeamID FROM UserTeam WHERE UserID = " + HttpContext.Session.GetString("userid");
            SqlDataReader teamNameReader = DBClass.GeneralReaderQuery(teamNameQuery);
            if (teamNameReader.Read())
            {
                HasTeam = true;
            }

            return Page();
        }
    }
}
