using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System.Text.RegularExpressions;

namespace EventManagementSystem.Pages.Users
{
    public class IndexModel : PageModel
    {
        public List<User> Users { get; set; }

        [BindProperty]
        public string? InputString { get; set; }

        public string[]? Keywords { get; set; }

        [BindProperty]
        public bool test { get; set; }

        public IndexModel()
        {
            Users = new List<User>();
            test = false;
        }

        public IActionResult OnGet(string User)
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

            string sqlQuery = "SELECT [User].UserID, concat_ws(' ', [User].FirstName, [User].LastName) as FullName, [User].Email, [User].PhoneNumber, [User].Username, Allergy.Category, " +
                            "[User].Accomodation, [User].IsActive, Role.Name FROM Allergy INNER JOIN " +
                            "[User] ON Allergy.AllergyID = [User].AllergyID INNER JOIN UserRole ON [User].UserID = UserRole.UserID INNER JOIN " +
                            "Role ON UserRole.RoleID = Role.RoleID ORDER BY [User].IsActive DESC, Role.Name ASC";

            SqlDataReader userReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (userReader.Read())
            {
                Users.Add(new User
                {
                    UserID = Int32.Parse(userReader["UserID"].ToString()),
                    FirstName = userReader["FullName"].ToString(),
                    Email = userReader["Email"].ToString(),
                    PhoneNumber = userReader["PhoneNumber"].ToString(),
                    Username = userReader["Username"].ToString(),
                    Accomodation = userReader["Category"].ToString(),
                    IsActive = Boolean.Parse(userReader["IsActive"].ToString()),
                    RoleType = userReader["Name"].ToString()
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

            for (int i = 0; i < Keywords.Length; i++)
            {
                keyword = Keywords[i];

                // query to do a CASE INSENSITIVE search for a keyword in the Event Table 
                sqlQuery = "SELECT [User].UserID, concat_ws(' ', [User].FirstName, [User].LastName) as FullName, [User].Email, [User].PhoneNumber, [User].Username, Allergy.Category, " +
                            "[User].Accomodation, [User].IsActive, Role.Name FROM Allergy INNER JOIN " +
                            "[User] ON Allergy.AllergyID = [User].AllergyID INNER JOIN UserRole ON[User].UserID = UserRole.UserID INNER JOIN " +
                            "Role ON UserRole.RoleID = Role.RoleID " +
                            "WHERE [User].FirstName LIKE '%" + keyword + "%' OR [User].LastName LIKE '%" + keyword + "%' " +
                            "OR [User].Username LIKE '%" + keyword + "%' ORDER BY UserID DESC";

                SqlDataReader userReader = DBClass.GeneralReaderQuery(sqlQuery);

                while (userReader.Read())
                {
                    Users.Add(new User
                    {
                        UserID = Int32.Parse(userReader["UserID"].ToString()),
                        FirstName = userReader["FullName"].ToString(),
                        Email = userReader["Email"].ToString(),
                        PhoneNumber = userReader["PhoneNumber"].ToString(),
                        Username = userReader["Username"].ToString(),
                        Accomodation = userReader["Category"].ToString(),
                        IsActive = Boolean.Parse(userReader["IsActive"].ToString()),
                        RoleType = userReader["Name"].ToString()
                    });
                }
            }
            DBClass.DBConnection.Close();

            return Page();
        }
    }
}
