using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Net.Http.Headers;

namespace EventManagementSystem.Pages.Users
{
    public class EditUsersModel : PageModel
    {
        [BindProperty]
        public User UserToUpdate { get; set; }

        public EditUsersModel()
        {
            UserToUpdate = new User();
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
                UserToUpdate.UserID = userid;
                UserToUpdate.FirstName = singleUser["FirstName"].ToString();
                UserToUpdate.LastName = singleUser["LastName"].ToString();
                UserToUpdate.Username = singleUser["Username"].ToString();
                UserToUpdate.Email = singleUser["Email"].ToString();
                UserToUpdate.PhoneNumber = singleUser["PhoneNumber"].ToString();
                UserToUpdate.IsAttendee = (bool)singleUser["IsAttendee"];
                UserToUpdate.IsPresenter = (bool)singleUser["IsPresenter"];
                UserToUpdate.IsAdmin = (bool)singleUser["IsAdmin"];
            }

            DBClass.LabDBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            string userQuery = "UPDATE \"User\" SET FirstName='" + UserToUpdate.FirstName
                + "',LastName='" + UserToUpdate.LastName
                + "',Username='" + UserToUpdate.Username
                + "',Email='" + UserToUpdate.Email
                + "',PhoneNumber='" + UserToUpdate.PhoneNumber
                + "',IsAttendee='" + UserToUpdate.IsAttendee
                + "',IsPresenter='" + UserToUpdate.IsPresenter
                + "',IsAdmin='" + UserToUpdate.IsAdmin
                + "' WHERE UserID=" + UserToUpdate.UserID;

            DBClass.GeneralQuery(userQuery);
            DBClass.LabDBConnection.Close();

            // case that the user has inputted a new password for the user (the input is NOT blank)
            if (UserToUpdate.Password != null)
            {
                string newPassword = PasswordHash.HashPassword(UserToUpdate.Password);

                string credentialsQuery = "UPDATE HashedCredentials SET Password='" + newPassword +
                    "' WHERE UserID=" + UserToUpdate.UserID;

                DBClass.AuthGeneralQuery(credentialsQuery);
                DBClass.LabDBConnection.Close();
            }

            return RedirectToPage("Index");
        }

    }
}
