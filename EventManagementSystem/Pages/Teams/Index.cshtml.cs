using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Teams
{
    public class IndexModel : PageModel
    {
        public List<Team> Teams { get; set; }

        public IndexModel()
        {
            Teams = new List<Team>();
        }

        public IActionResult OnGet()
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

            string sqlQuery = "SELECT * FROM Team";
            SqlDataReader teamsReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (teamsReader.Read())
            {
                Teams.Add(new Team
                {
                    TeamID = Int32.Parse(teamsReader["TeamID"].ToString()),
                    Name = teamsReader["Name"].ToString(),
                    Description = teamsReader["Description"].ToString(),
                    MaxSize = Int32.Parse(teamsReader["MaxSize"].ToString())
                });
            }

            DBClass.DBConnection.Close();

            return Page();
        }
    }
}