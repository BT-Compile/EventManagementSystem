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
            if (HttpContext.Session.GetString("usertype") != "Admin" && HttpContext.Session.GetString("usertype") == "Attendee")
            {
                return RedirectToPage("/Attendee/Index");
            }
            else if (HttpContext.Session.GetString("usertype") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            string sqlQuery = "SELECT * FROM \"User\"";

            SqlDataReader userReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (userReader.Read())
            {
                Users.Add(new User
                {
                    UserID = Int32.Parse(userReader["UserID"].ToString()),
                    FirstName = userReader["FirstName"].ToString(),
                    LastName = userReader["LastName"].ToString(),
                    Username = userReader["Username"].ToString(),
                    Email = userReader["Email"].ToString(),
                    PhoneNumber = userReader["PhoneNumber"].ToString(),
                    IsAttendee = (bool)userReader["IsAttendee"],
                    IsPresenter = (bool)userReader["IsPresenter"],
                    IsAdmin = (bool)userReader["IsAdmin"],
                    IsActive = (bool)userReader["IsActive"]
                });
            }

            DBClass.DBConnection.Close();

            return Page();
        }
    }
}
