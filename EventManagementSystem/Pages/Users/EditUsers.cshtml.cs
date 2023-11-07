using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

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
                UserToUpdate.AllergyNote = singleUser["AllergyNote"].ToString();
            }
            DBClass.DBConnection.Close();

            // Retrieves the role name from the Role table for the corresponding User that was selected
            string roleNameQuery = "SELECT [Role].[Name] AS roleName " +
                "FROM [Role] " +
                "INNER JOIN UserRole ON [Role].RoleID = UserRole.RoleID " +
                "WHERE UserRole.UserID = " + UserToUpdate.UserID;
            SqlDataReader roleIDReader = DBClass.GeneralReaderQuery(roleNameQuery);
            if (roleIDReader.Read())
            {
                UserToUpdate.RoleType = roleIDReader["roleName"].ToString();
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
                + "',AllergyNote='" + UserToUpdate.AllergyNote
                + "' WHERE UserID=" + UserToUpdate.UserID;

            DBClass.GeneralQuery(userQuery);
            DBClass.DBConnection.Close();

            // Retrieves the RoleID from the Role table for the corresponding RoleType that was selected
            string roleQuery = "SELECT RoleID FROM [Role] WHERE Name='" + UserToUpdate.RoleType + "'";
            SqlDataReader roleIDReader = DBClass.GeneralReaderQuery(roleQuery);
            int roleID = 0;
            if (roleIDReader.Read())
            {
                roleID = Int32.Parse(roleIDReader["RoleID"].ToString());
            }
            DBClass.DBConnection.Close();

            // Update UserRole table
            string userRoleQuery = "UPDATE UserRole SET RoleID = '" + roleID + "', AssignDate = GETDATE() WHERE UserID = " + UserToUpdate.UserID;
            DBClass.GeneralQuery(userRoleQuery);
            DBClass.DBConnection.Close();

            // case that the user has inputted a new password for the user (the input is NOT blank)
            if (UserToUpdate.UserPassword != null)
            {
                string newPassword = PasswordHash.HashPassword(UserToUpdate.UserPassword);

                string credentialsQuery = "UPDATE HashedCredentials SET HashedPassword='" + newPassword +
                    "' WHERE UserID=" + UserToUpdate.UserID;

                DBClass.AuthGeneralQuery(credentialsQuery);
                DBClass.DBConnection.Close();
            }

            return RedirectToPage("Index");
        }

    }
}
