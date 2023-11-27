using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Users
{
    public class UserProfileModel : PageModel
    {
        [BindProperty]
        public List<EventUser> Users { get; set; }

        [BindProperty]
        public User UsersTable { get; set; }

        public UserProfileModel()
        {
            Users = new List<EventUser>();
            UsersTable = new User();
        }

        public IActionResult OnGet(int userid)
        {
            if (HttpContext.Session.GetString("RoleType") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            /*
            string sqlQuery = "SELECT Event.*, [User].UserID, concat_ws(' ', [User].FirstName, [User].LastName) as FullName, [User].Email, [User].PhoneNumber, [User].Username, " +
                              "[User].Accomodation, [User].IsActive, Role.*, Allergy.Category, EventRegister.RegistrationDate FROM  Event INNER JOIN EventRegister ON Event.EventID " +
                              "= EventRegister.EventID INNER JOIN [User] ON EventRegister.UserID = [User].UserID INNER JOIN UserRole ON [User].UserID = UserRole.UserID INNER JOIN Role " +
                              "ON UserRole.RoleID = Role.RoleID LEFT OUTER JOIN Allergy ON [User].AllergyID = Allergy.AllergyID LEFT OUTER JOIN AllergyBridge ON [User].UserID = AllergyBridge.UserID " +
                              "AND Allergy.AllergyID = AllergyBridge.AllergyID WHERE [User].UserID = " + userid +
                               " AND [User].IsActive = 'true'";
            */

            string sqlQuery = "SELECT Event.*, Role.*, [User].UserID, EventRegister.RegistrationDate, concat_ws(' ', [User].FirstName, [User].LastName) as FullName FROM  Event INNER JOIN EventRegister ON Event.EventID = EventRegister.EventID LEFT OUTER JOIN " +
                              "Role ON EventRegister.RoleID = Role.RoleID LEFT OUTER JOIN [User] ON EventRegister.UserID = [User].UserID LEFT OUTER JOIN " +
                              "UserRole ON Role.RoleID = UserRole.RoleID AND[User].UserID = UserRole.UserID WHERE [User].UserID =  " + userid + " ORDER BY Event.StartDate ASC";

            SqlDataReader userReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (userReader.Read()) //Not reading for admin or organizer roles need to troubleshoot
            {
                Users.Add(new EventUser
                {
                    UserID = Int32.Parse(userReader["UserID"].ToString()),
                    FirstName = userReader["FullName"].ToString(),
                    RoleType = userReader["Name"].ToString(),
                    EventID = Int32.Parse(userReader["EventID"].ToString()),
                    EventName = userReader["EventName"].ToString(),
                    RegistrationDate = (DateTime)userReader["RegistrationDate"],
                    StartDate = (DateTime)userReader["StartDate"]
                });
            }
            DBClass.DBConnection.Close();

            sqlQuery = "SELECT [User].UserID, concat_ws(' ', [User].FirstName, [User].LastName) as FullName, [User].Email, [User].PhoneNumber, [User].Username, Allergy.Category, " +
                            "[User].Accomodation, [User].IsActive, Role.Name FROM Allergy INNER JOIN " +
                            "[User] ON Allergy.AllergyID = [User].AllergyID INNER JOIN UserRole ON[User].UserID = UserRole.UserID INNER JOIN " +
                            "Role ON UserRole.RoleID = Role.RoleID WHERE [User].UserID = " + userid + "ORDER BY [User].IsActive DESC, Role.Name ASC";

            SqlDataReader usertableReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (usertableReader.Read())
            {
                UsersTable.UserID = Int32.Parse(usertableReader["UserID"].ToString());
                UsersTable.FirstName = usertableReader["FullName"].ToString();
                UsersTable.Email = usertableReader["Email"].ToString();
                UsersTable.PhoneNumber = usertableReader["PhoneNumber"].ToString();
                UsersTable.Username = usertableReader["Username"].ToString();
                UsersTable.Category = usertableReader["Category"].ToString();
                UsersTable.Accomodation = usertableReader["Accomodation"].ToString();
                UsersTable.IsActive = Boolean.Parse(usertableReader["IsActive"].ToString());
                UsersTable.RoleType = usertableReader["Name"].ToString();
            }

            DBClass.DBConnection.Close();

            return Page();
        }
    }
}
