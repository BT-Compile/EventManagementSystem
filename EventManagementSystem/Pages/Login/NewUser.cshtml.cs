using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EventManagementSystem.Pages.Login
{
    // THIS CLASS IS SPECIFICALLY USED BY ATTENDEE USERS TRYING TO CREATE A NEW USER ACCOUNT
    public class NewUserModel : PageModel
    {
        [BindProperty]
        public User UserToCreate { get; set; }

        public NewUserModel()
        {
            UserToCreate = new User();
        }

        // No User is needed to get in this method,
        // we are not updating a User, only creating a new one
        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {
            DBClass.SecureUserCreation(UserToCreate.FirstName, UserToCreate.LastName, UserToCreate.Email,
                UserToCreate.PhoneNumber, UserToCreate.Username, UserToCreate.AllergyNote, UserToCreate.Accessibility, UserToCreate.IsActive);

            DBClass.DBConnection.Close();

            DBClass.CreateHashedUser(UserToCreate.Username, UserToCreate.UserPassword);

            DBClass.DBConnection.Close();

            return RedirectToPage("Index");
        }
    }
}
