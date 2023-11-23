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

        [BindProperty]
        public string UserType { get; set; }

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

                sqlQuery = "SELECT * FROM [User] WHERE Username = '" + HttpContext.Session.GetString("username") + "';";
                SqlDataReader singleUser = DBClass.GeneralReaderQuery(sqlQuery);
                singleUser.Read();

                string UserID = singleUser["UserID"].ToString();
                HttpContext.Session.SetString("userid", UserID);

                sqlQuery = "SELECT RoleID FROM UserRole WHERE UserID = '" + UserID + "'";
                SqlDataReader isAdminReader = DBClass.GeneralReaderQuery(sqlQuery);
                isAdminReader.Read();

                // Case that an Admin has logged in
                if (Int32.Parse(isAdminReader["RoleID"].ToString()) == 1)
                {
                    DBClass.DBConnection.Close();
                    HttpContext.Session.SetString("RoleType", "Admin");
                    return RedirectToPage("/Admin/Index");
                }
                // Case that a Presenter User has logged in
                else if (Int32.Parse(isAdminReader["RoleID"].ToString()) == 2)
                {
                    DBClass.DBConnection.Close();
                    HttpContext.Session.SetString("RoleType", "Presenter");
                    return RedirectToPage("/Presenter/Index");
                }
                // Case that a Judge User has logged in
                else if (Int32.Parse(isAdminReader["RoleID"].ToString()) == 3)
                {
                    DBClass.DBConnection.Close();
                    HttpContext.Session.SetString("RoleType", "Judge");
                    return RedirectToPage("/Judge/Index");
                }
                // Case that a Participant (Attendee) User has logged in
                else if (Int32.Parse(isAdminReader["RoleID"].ToString()) == 4)
                {
                    DBClass.DBConnection.Close();
                    HttpContext.Session.SetString("RoleType", "Participant");
                    return RedirectToPage("/Attendee/Index");
                }
                // Case that a Organizer User has logged in
                else if (Int32.Parse(isAdminReader["RoleID"].ToString()) == 5)
                {
                    DBClass.DBConnection.Close();
                    HttpContext.Session.SetString("RoleType", "Organizer");
                    return RedirectToPage("/Organizer/Index");
                }
            }
            else // Case where a User with matching credentials (against their encrypted password) has not been found
            {
                ViewData["LoginMessage"] = "Username and/or Password Incorrect";
            }

            DBClass.DBConnection.Close();

            return Page();

        }
    }
}
