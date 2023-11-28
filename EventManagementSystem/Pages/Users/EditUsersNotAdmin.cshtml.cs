using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Users
{
    public class EditUsersNotAdminModel : PageModel
    {
        [BindProperty]
        public User UserToUpdate { get; set; }

        public List<SelectListItem> Roles { get; set; }

        public List<SelectListItem> Allergies { get; set; }

        public EditUsersNotAdminModel()
        {
            UserToUpdate = new User();
        }

        public IActionResult OnGet(int userid)
        {
            if (HttpContext.Session.GetString("RoleType") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            // Read data from the User table into each field
            // NOTE: This excludes the "RoleType" field because that exists in the associative table
            SqlDataReader singleUser = DBClass.SingleUserReader(userid);
            while (singleUser.Read())
            {
                UserToUpdate.UserID = userid;
                UserToUpdate.FirstName = singleUser["FirstName"].ToString();
                UserToUpdate.LastName = singleUser["LastName"].ToString();
                UserToUpdate.Email = singleUser["Email"].ToString();
                UserToUpdate.PhoneNumber = singleUser["PhoneNumber"].ToString();
                UserToUpdate.Username = singleUser["Username"].ToString();
                UserToUpdate.AllergyID = singleUser["AllergyID"].ToString();
            }
            DBClass.DBConnection.Close();

            SqlDataReader AllergyReader = DBClass.GeneralReaderQuery("SELECT * FROM Allergy");
            Allergies = new List<SelectListItem>();
            while (AllergyReader.Read())
            {
                Allergies.Add(new SelectListItem(
                    AllergyReader["Category"].ToString(),
                    AllergyReader["AllergyID"].ToString()));
            }
            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            // Update User table
            string userQuery = "UPDATE \"User\" SET FirstName='" + UserToUpdate.FirstName
                + "',LastName='" + UserToUpdate.LastName
                + "',Username='" + UserToUpdate.Username
                + "',Email='" + UserToUpdate.Email
                + "',PhoneNumber='" + UserToUpdate.PhoneNumber
                + "',AllergyID='" + UserToUpdate.AllergyID
                + "' WHERE UserID=" + UserToUpdate.UserID;

            DBClass.GeneralQuery(userQuery);
            DBClass.DBConnection.Close();

            return RedirectToPage("Index");
        }

    }
}
