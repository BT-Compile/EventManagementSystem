using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Teams
{
    public class EditTeamModel : PageModel
    {
        [BindProperty]
        public Team TeamToUpdate { get; set; }

        public EditTeamModel()
        {
            TeamToUpdate = new Team();
        }

        public IActionResult OnGet(int teamid)
        {
            if (HttpContext.Session.GetString("RoleType") != "Admin" &&
                (HttpContext.Session.GetString("RoleType") == "Presenter" || HttpContext.Session.GetString("RoleType") == "Judge"
                || HttpContext.Session.GetString("RoleType") == "Participant" || HttpContext.Session.GetString("RoleType") == "Organizer"))
            {
                return RedirectToPage("/Attendee/Index");
            }
            else if (HttpContext.Session.GetString("RoleType") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            // Read data from the Team table into each field
            SqlDataReader singleTeam = DBClass.SingleTeamReader(teamid);
            if (singleTeam.Read())
            {
                TeamToUpdate.TeamID = teamid;
                TeamToUpdate.Name = singleTeam["Name"].ToString();
                TeamToUpdate.Description = singleTeam["Description"].ToString();
                TeamToUpdate.MaxSize = Int32.Parse(singleTeam["MaxSize"].ToString());
            }
            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery = "UPDATE TEAM SET " +
                "Name = '" + TeamToUpdate.Name +
                "', Description = '" + TeamToUpdate.Description +
                "', MaxSize = " + TeamToUpdate.MaxSize + 
                " WHERE TeamID = " + TeamToUpdate.TeamID;
            DBClass.GeneralQuery(sqlQuery);
            DBClass.DBConnection.Close();

            return RedirectToPage("Index");
        }

    }
}
