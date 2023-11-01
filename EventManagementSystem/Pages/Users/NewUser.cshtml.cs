using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Runtime.CompilerServices;

namespace EventManagementSystem.Pages.Users
{
    // THIS CLASS IS SPECIFICALLY USED BY ADMIN USERS CREATING A USER ACCOUNT
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

            return Page();
        }

        public IActionResult OnPost()
        {
            DBClass.SecureUserCreation(UserToCreate.FirstName, UserToCreate.LastName, UserToCreate.Username, UserToCreate.Email,
                UserToCreate.PhoneNumber, UserToCreate.IsAttendee, UserToCreate.IsPresenter, UserToCreate.IsAdmin);
            
            DBClass.DBConnection.Close();

            DBClass.CreateHashedUser(UserToCreate.Username, UserToCreate.UserPassword);

            DBClass.DBConnection.Close();

            return RedirectToPage("Index");
        }
    }
}
