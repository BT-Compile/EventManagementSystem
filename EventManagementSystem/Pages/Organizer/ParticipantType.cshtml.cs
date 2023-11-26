using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace EventManagementSystem.Pages.Organizer
{
    public class ParticipantTypeModel : PageModel
    {
        public List<EventUser> Users { get; set; }

        [BindProperty]
        public string? InputString { get; set; }

        public string[]? Keywords { get; set; }

        [BindProperty]
        public bool test { get; set; }

        public ParticipantTypeModel()
        {
            Users = new List<EventUser>();
            test = false;
        }

        public IActionResult OnGet(int eventid)
        {
            if (HttpContext.Session.GetString("RoleType") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            HttpContext.Session.SetInt32("eventid", eventid);

            string sqlQuery = "SELECT * FROM [Role] WHERE Name != 'Admin' AND Name != 'Organizer' AND Name != 'Participant'";

            SqlDataReader userReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (userReader.Read())
            {
                Users.Add(new EventUser
                {
                    RoleID = Int32.Parse(userReader["RoleID"].ToString()),
                    RoleName = userReader["Name"].ToString(),
                    RoleDescription = userReader["Description"].ToString()
                });
            }

            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost(string keyword)
        {
            test = true;
            Keywords = Regex.Split(InputString, @"\s+");
            string sqlQuery;
            int? eventid = HttpContext.Session.GetInt32("eventid");

            for (int i = 0; i < Keywords.Length; i++)
            {
                keyword = Keywords[i];

                sqlQuery = "SELECT * FROM [Role] WHERE (Name != 'Admin' AND Name != 'Organizer' AND Name != 'Participant') AND (Name LIKE '%" + keyword + "%' OR Description LIKE '%" + keyword + "%')";

                SqlDataReader userReader = DBClass.GeneralReaderQuery(sqlQuery);

                while (userReader.Read())
                {
                    Users.Add(new EventUser
                    {
                        RoleID = Int32.Parse(userReader["RoleID"].ToString()),
                        RoleName = userReader["Name"].ToString(),
                        RoleDescription = userReader["Description"].ToString()
                    });
                }
            }

            DBClass.DBConnection.Close();
            return Page();
        }
    }
}
