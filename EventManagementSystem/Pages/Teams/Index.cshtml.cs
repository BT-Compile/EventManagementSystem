using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace EventManagementSystem.Pages.Teams
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public bool HasPosted { get; set; }

        [BindProperty]
        public string? InputString { get; set; }

        public string[]? Keywords { get; set; }

        public List<Team> Teams { get; set; }

        public IndexModel()
        {
            Teams = new List<Team>();
            HasPosted = false;
        }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("RoleType") == null)
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

        public IActionResult OnPost()
        {
            HasPosted = true;
            Keywords = Regex.Split(InputString, @"\s+");
            string keyword, sqlQuery;

            for (int i = 0; i < Keywords.Length; i++)
            {
                keyword = Keywords[i];

                // query to do a CASE INSENSITIVE search for a keyword in the Teams table 
                sqlQuery = "SELECT * FROM Team WHERE ([Name] LIKE '%" + keyword + "%' OR [Description] LIKE'%" + keyword + "%')";
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
            }
            return Page();
        }
    }
}
