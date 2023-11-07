using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Users
{
    public class IndexModel : PageModel
    {
        public List<User> Users { get; set; }

        public IndexModel()
        {
            Users = new List<User>();
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

            string sqlQuery = "SELECT [User].*, [Role].[Name] AS RoleName " +
                "FROM [User] " +
                "LEFT JOIN UserRole ON [User].UserID = UserRole.UserID " +
                "LEFT JOIN [Role] ON UserRole.RoleID = [Role].RoleID;";

            SqlDataReader userReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (userReader.Read())
            {
                Users.Add(new User
                {
                    UserID = Int32.Parse(userReader["UserID"].ToString()),
                    FirstName = userReader["FirstName"].ToString(),
                    LastName = userReader["LastName"].ToString(),
                    Email = userReader["Email"].ToString(),
                    PhoneNumber = userReader["PhoneNumber"].ToString(),
                    Username = userReader["Username"].ToString(),
                    AllergyNote = userReader["AllergyNote"].ToString(),
                    RoleType = userReader["RoleName"].ToString(),
                    //IsActive = (bool)userReader["IsActive"] IMPLEMENT LATER -SR
                }); 
            }

            DBClass.DBConnection.Close();

            return Page();
        }
    }
}
