using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System.Text.RegularExpressions;

namespace EventManagementSystem.Pages.Attendee.LeaveTeam
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string? InputString { get; set; }

        public string[]? Keywords { get; set; }

        public List<Team> Teams { get; set; }

        public IndexModel()
        {
            Teams = new List<Team>();
        }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            // Retrieve all of the teams that the user is signed up for
            string teamsQuery = "SELECT T.* " +
                "FROM Team T " +
                "JOIN UserTeam UT ON T.TeamID = UT.TeamID " +
                "JOIN [User] U ON UT.UserID = U.UserID " +
                "WHERE U.UserName = '" + HttpContext.Session.GetString("username") + "'";
            SqlDataReader teamsReader = DBClass.GeneralReaderQuery(teamsQuery);
            while (teamsReader.Read())
            {
                Teams.Add(new Team
                {
                    TeamID = Int32.Parse(teamsReader["TeamID"].ToString()),
                    Name = teamsReader["Name"].ToString(),
                    Description = teamsReader["Description"].ToString()
                });
            }

            return Page();
        }

        public IActionResult OnPost(string keyword)
        {
            // SEARCH DOES NOT WORK
            Keywords = Regex.Split(InputString, @"\s+");
            string sqlQuery;

            for (int i = 0; i < Keywords.Length; i++)
            {
                keyword = Keywords[i];

                sqlQuery = "SELECT Team.TeamID, Team.Name, Team.Description, Team.MaxSize " +
                "FROM Team " +
                "WHERE Team.TeamID NOT IN (" +
                "   SELECT UserTeam.TeamID " +
                "   FROM[User] " +
                "   INNER JOIN UserTeam ON[User].UserID = UserTeam.UserID " +
                "   WHERE[User].Username = '" + HttpContext.Session.GetString("username") + "') " +
                "AND (Team.Name LIKE '%" + keyword + "%' OR Team.Description LIKE '%" + keyword + "%')";

                SqlDataReader userReader = DBClass.GeneralReaderQuery(sqlQuery);

                SqlDataReader teamNamesReader = DBClass.GeneralReaderQuery(sqlQuery);
                while (teamNamesReader.Read())
                {
                    Teams.Add(new Team
                    {
                        TeamID = Int32.Parse(teamNamesReader["TeamID"].ToString()),
                        Name = teamNamesReader["Name"].ToString(),
                        Description = teamNamesReader["Description"].ToString()
                    });
                }
            }
            DBClass.DBConnection.Close();

            return Page();
        }
    }
}
