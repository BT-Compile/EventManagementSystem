using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace EventManagementSystem.Pages.Users
{
    // THIS CLASS IS SPECIFICALLY USED BY ADMIN USERS CREATING A USER ACCOUNT
    public class NewUserModel : PageModel
    {
        [BindProperty]
        public User UserToCreate { get; set; }

        public List<SelectListItem> Roles { get; set; }

        public NewUserModel()
        {
            UserToCreate = new User();
        }

        // No User is needed to get in this method,
        // we are not updating a User, only creating a new one
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

            // Populate the roles select control
            SqlDataReader RolesReader = DBClass.GeneralReaderQuery("SELECT * FROM [Role]");
            Roles = new List<SelectListItem>();
            while (RolesReader.Read())
            {
                Roles.Add(new SelectListItem(
                    RolesReader["Name"].ToString(),
                    RolesReader["RoleID"].ToString()));
            }
            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            DBClass.SecureUserCreation(UserToCreate.FirstName, UserToCreate.LastName, UserToCreate.Email,
                UserToCreate.PhoneNumber, UserToCreate.Username, UserToCreate.AllergyID, UserToCreate.Accomodation, UserToCreate.IsActive);
            DBClass.DBConnection.Close();

            DBClass.CreateHashedUser(UserToCreate.Username, UserToCreate.UserPassword);
            DBClass.DBConnection.Close();

            // Query to retrieve the userID of the User we just created.
            string lastUserIDQuery = "SELECT MAX(UserID) AS LastUserID FROM [User]";
            SqlDataReader lastUserIDReader = DBClass.GeneralReaderQuery(lastUserIDQuery);
            int lastUserID = 0;
            if (lastUserIDReader.Read())
            {
                lastUserID = Int32.Parse(lastUserIDReader["LastUserID"].ToString());
            }
            DBClass.DBConnection.Close();

            // MAX(UserID) is used to identify the most recently created user (inserted above)
            string userRoleInsert = "INSERT INTO UserRole (UserID, RoleID, AssignDate) VALUES " +
                "(" + lastUserID + ", " + UserToCreate.RoleType + ", GETDATE())";
            DBClass.GeneralQuery(userRoleInsert);
            DBClass.DBConnection.Close();

            return RedirectToPage("Index");
        }
    }
}
