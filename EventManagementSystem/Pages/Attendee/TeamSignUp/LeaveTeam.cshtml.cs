using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Attendee.TeamSignUp
{
    public class LeaveTeamModel : PageModel
    {
        [BindProperty]
        public User UserToLeave { get; set; }

        [BindProperty]
        public Team TeamToLeave { get; set; }

        public LeaveTeamModel()
        {
            UserToLeave = new User();
            TeamToLeave = new Team();
        }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            string sqlQuery;

            // Query to get the full name and userid of the current user
            sqlQuery = "SELECT * FROM \"User\" " +
                "WHERE Username = '" + HttpContext.Session.GetString("username") + "';";
            SqlDataReader singleUser = DBClass.GeneralReaderQuery(sqlQuery);

            while (singleUser.Read())
            {
                UserToLeave.UserID = Int32.Parse(singleUser["UserID"].ToString());
                UserToLeave.FirstName = singleUser["FirstName"].ToString();
                UserToLeave.LastName = singleUser["LastName"].ToString();
            }
            DBClass.DBConnection.Close();

            // Query to get the teamid of the team the user the a part of
            sqlQuery = "SELECT * FROM UserTeam WHERE UserID = " + HttpContext.Session.GetString("userid");
            SqlDataReader userTeam = DBClass.GeneralReaderQuery(sqlQuery);

            while (userTeam.Read())
            {
                HttpContext.Session.SetString("teamid", userTeam["TeamID"].ToString());
            }

            // Query to get the team name and teamid that the user wants to leave
            sqlQuery = "SELECT * FROM Team WHERE TeamID = " + HttpContext.Session.GetString("teamid");
            SqlDataReader singleTeam = DBClass.GeneralReaderQuery(sqlQuery);

            while (singleTeam.Read())
            {
                TeamToLeave.Name = singleTeam["Name"].ToString();
            }
            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery = "DELETE FROM UserTeam " +
                "WHERE UserID = " + HttpContext.Session.GetString("userid") + " AND TeamID = " + HttpContext.Session.GetString("teamid");
            DBClass.GeneralQuery(sqlQuery);

            HttpContext.Session.Remove("teamid");

            return RedirectToPage("../Index");
        }
    }
}
