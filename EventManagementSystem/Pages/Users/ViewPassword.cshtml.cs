using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Users
{
    public class ViewPasswordModel : PageModel
    {
        [BindProperty]
        public User User { get; set; }

        [BindProperty]
        public string EncryptedPassword { get; set; }

        public ViewPasswordModel()
        {
            User = new User();
        }

        public IActionResult OnGet(int userid)
        {
            if (HttpContext.Session.GetString("usertype") != "Admin" && HttpContext.Session.GetString("usertype") == "Attendee")
            {
                return RedirectToPage("/Attendee/Index");
            }
            else if (HttpContext.Session.GetString("usertype") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            SqlDataReader singleUser = DBClass.SingleUserReader(userid);

            while (singleUser.Read())
            {
                User.FirstName = singleUser["FirstName"].ToString();
                User.LastName = singleUser["LastName"].ToString();
            }

            DBClass.DBConnection.Close();

            EncryptedPassword = DBClass.EncryptedPasswordReader(userid);

            return Page();
        }
    }
}
