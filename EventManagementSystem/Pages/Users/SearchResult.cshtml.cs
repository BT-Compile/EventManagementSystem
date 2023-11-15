using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Users
{
    public class SearchResultModel : PageModel
    {
        public List<User> Users { get; set; }

        public SearchResultModel()
        {
            Users = new List<User>();
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
                            "[User] ON Allergy.AllergyID = [User].AllergyID INNER JOIN UserRole ON[User].UserID = UserRole.UserID INNER JOIN " +
                            "Role ON UserRole.RoleID = Role.RoleID WHERE concat_ws(' ', [User].FirstName, [User].LastName) LIKE '%" + User + "%'";

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
                    Accomodation = userReader["Accomodation"].ToString(),
                    Category = userReader["Category"].ToString(),
                    IsActive = Boolean.Parse(userReader["IsActive"].ToString()),
                    RoleType = userReader["Name"].ToString()
                });
            }

            DBClass.DBConnection.Close();

            return Page();
        }
    }
}
