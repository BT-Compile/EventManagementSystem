using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Login
{
    // THIS CLASS IS SPECIFICALLY USED BY ATTENDEE USERS TRYING TO CREATE A NEW USER ACCOUNT
    public class NewUserModel : PageModel
    {
        [BindProperty]
        public User UserToCreate { get; set; }

        [BindProperty]
        public bool IsTaken { get; set; }

        public int ParticipantRole { get; set; }

        public List<SelectListItem> Allergies { get; set; }

        public List<User> UsernameFinder { get; set; }

        public NewUserModel()
        {
            UserToCreate = new User();
            UsernameFinder = new List<User>();
            IsTaken = false;
        }

        // No User is needed to get in this method,
        // we are not updating a User, only creating a new one
        public void OnGet()
        {
            SqlDataReader AllergyReader = DBClass.GeneralReaderQuery("SELECT * FROM Allergy");
            Allergies = new List<SelectListItem>();
            while (AllergyReader.Read())
            {
                Allergies.Add(new SelectListItem(
                    AllergyReader["Category"].ToString(),
                    AllergyReader["AllergyID"].ToString()));
            }
            DBClass.DBConnection.Close();
        }

        public IActionResult OnPost()
        {

            SqlDataReader scheduleReader = DBClass.GeneralReaderQuery("SELECT Username FROM [User]");

            while (scheduleReader.Read())
            {
                UsernameFinder.Add(new User
                {
                    Username = scheduleReader["Username"].ToString()
                });
            }
            DBClass.DBConnection.Close();

            foreach (var user in UsernameFinder)
            {
                if (user.Username == UserToCreate.Username)
                {
                    HttpContext.Session.SetString("IsTaken", "yes");
                    IsTaken = true;
                    ViewData["temp"] = "Username Already Taken.";

                    break;
                }
            }

            if (IsTaken == true)
            {
                return RedirectToPage("NewUser");
            }

            else
            {

                DBClass.SecureUserCreation(UserToCreate.FirstName, UserToCreate.LastName, UserToCreate.Email,
                UserToCreate.PhoneNumber, UserToCreate.Username, UserToCreate.Accomodation, UserToCreate.AllergyID);
                DBClass.DBConnection.Close();

                DBClass.CreateHashedUser(UserToCreate.Username, UserToCreate.UserPassword);
                DBClass.DBConnection.Close();

                DBClass.NewUserParticipantAssign(UserToCreate.Username);
                DBClass.DBConnection.Close();

                return RedirectToPage("Index");
            }
        }
    }
}
