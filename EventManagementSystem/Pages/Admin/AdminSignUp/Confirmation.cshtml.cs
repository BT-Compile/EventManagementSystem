using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.SignUp
{
    public class ConfirmationModel : PageModel
    {
        [BindProperty]
        public User UserToSignUp { get; set; }

        [BindProperty]
        public Activity ActivityToSignUp { get; set; }

        public ConfirmationModel()
        {
            UserToSignUp = new User();
            ActivityToSignUp = new Activity();
        }

        public IActionResult OnGet(int userid, int activityid)
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

            string sqlQuery;

            sqlQuery = "SELECT * FROM \"User\" WHERE UserID = " + userid;
            SqlDataReader singleUser = DBClass.GeneralReaderQuery(sqlQuery);

            while (singleUser.Read())
            {
                UserToSignUp.UserID = userid;
                UserToSignUp.FirstName = singleUser["FirstName"].ToString();
                UserToSignUp.LastName = singleUser["LastName"].ToString();
            }

            DBClass.DBConnection.Close();

            sqlQuery = "SELECT * FROM Activity WHERE ActivityID = " + activityid;
            SqlDataReader singleActivity = DBClass.GeneralReaderQuery(sqlQuery);

            while (singleActivity.Read())
            {
                ActivityToSignUp.ActivityID = activityid;
                ActivityToSignUp.ActivityName = singleActivity["ActivityName"].ToString();
            }

            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery = "INSERT INTO Attendance (UserID, ActivityID) VALUES (" +
                UserToSignUp.UserID + "," + ActivityToSignUp.ActivityID + ")";

            DBClass.GeneralQuery(sqlQuery);

            DBClass.DBConnection.Close();

            return RedirectToPage("/Users/UserAssociations");
        }

    }
}
