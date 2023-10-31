using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Login
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            // Case where a User with matching credentials (against their encrypted password) has been found
            if (DBClass.HashedParameterLogin(Username, Password))
            {

                string sqlQuery;

                HttpContext.Session.SetString("username", Username);

                sqlQuery = "SELECT * FROM \"User\" WHERE Username = '" + HttpContext.Session.GetString("username") + "';";
                SqlDataReader singleUser = DBClass.GeneralReaderQuery(sqlQuery);
                singleUser.Read();

                string UserID = singleUser["UserID"].ToString();
                HttpContext.Session.SetString("userid", UserID);

                sqlQuery = "SELECT isAdmin FROM \"User\" WHERE Username = '" + Username + "'";
                SqlDataReader isAdminReader = DBClass.GeneralReaderQuery(sqlQuery);
                isAdminReader.Read();

                // Case that an Admin has logged in
                if ((bool)isAdminReader["isAdmin"] == true)
                {
                    DBClass.LabDBConnection.Close();
                    HttpContext.Session.SetString("usertype", "Admin");
                    return RedirectToPage("/Admin/Index");
                }
                else // Case that a normal User has logged in
                {
                    DBClass.LabDBConnection.Close();
                    HttpContext.Session.SetString("usertype", "Attendee");
                    return RedirectToPage("/Attendee/Index");
                }
            }
            else // Case where a User with matching credentials (against their encrypted password) has not been found
            {
                ViewData["LoginMessage"] = "Username and/or Password Incorrect";
            }

            DBClass.LabDBConnection.Close();

            return Page();

        }
    }
}
