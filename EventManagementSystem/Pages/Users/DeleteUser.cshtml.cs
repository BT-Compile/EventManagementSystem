using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Users
{
    public class DeleteUserModel : PageModel
    {
        [BindProperty]
        public User UserToDelete { get; set; }

        public DeleteUserModel()
        {
            UserToDelete = new User();
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
                UserToDelete.UserID = userid;
            }

            DBClass.LabDBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery = "UPDATE \"User\" SET IsActive = 0 WHERE UserID = " + UserToDelete.UserID;

            DBClass.GeneralQuery(sqlQuery);

            DBClass.LabDBConnection.Close();

            return RedirectToPage("Index");
        }
    }
}
