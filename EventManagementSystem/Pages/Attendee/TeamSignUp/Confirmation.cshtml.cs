using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Attendee.TeamSignUp
{
    public class ConfirmationModel : PageModel
    {
        [BindProperty]
        public User UserToJoin { get; set; }

        [BindProperty]
        public Team TeamToJoin { get; set; }

        public ConfirmationModel()
        {
            UserToJoin = new User();
            TeamToJoin = new Team();
        }

        public IActionResult OnGet(int teamid)
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
                UserToJoin.UserID = Int32.Parse(singleUser["UserID"].ToString());
                UserToJoin.FirstName = singleUser["FirstName"].ToString();
                UserToJoin.LastName = singleUser["LastName"].ToString();
            }
            DBClass.DBConnection.Close();

            // Query to get the team name and teamid that the user wants to join
            sqlQuery = "SELECT * FROM Team WHERE TeamID = " + teamid;
            SqlDataReader singleTeam = DBClass.GeneralReaderQuery(sqlQuery);

            while (singleTeam.Read())
            {
                TeamToJoin.TeamID = teamid;
                TeamToJoin.Name = singleTeam["Name"].ToString();
            }
            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery = "INSERT INTO UserTeam (UserID, TeamID, JoinDate) VALUES " +
                "(" + HttpContext.Session.GetString("userid") + ", " + TeamToJoin.TeamID + ", GETDATE())";
            DBClass.GeneralQuery(sqlQuery);
            
            return RedirectToPage("../Index");
        }
    }
}
