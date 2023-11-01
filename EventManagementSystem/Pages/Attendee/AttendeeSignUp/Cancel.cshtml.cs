using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Attendee.AttendeeSignUp
{
    public class CancelModel : PageModel
    {
        [BindProperty]
        public User UserToSignUp { get; set; }

        [BindProperty]
        public Activity ActivityToSignUp { get; set; }

        public CancelModel()
        {
            UserToSignUp = new User();
            ActivityToSignUp = new Activity();
        }

        public IActionResult OnGet(int activityid)
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            string sqlQuery;

            sqlQuery = "SELECT * FROM \"User\" " +
                "WHERE Username = '" + HttpContext.Session.GetString("username") + "';";
            SqlDataReader singleUser = DBClass.GeneralReaderQuery(sqlQuery);

            while (singleUser.Read())
            {
                UserToSignUp.UserID = Int32.Parse(singleUser["UserID"].ToString());
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
            string sqlQuery = "DELETE FROM Attendance " +
                "WHERE UserID = " + HttpContext.Session.GetString("userid") +
                " AND ActivityID = " + ActivityToSignUp.ActivityID;
            DBClass.GeneralQuery(sqlQuery);

            DBClass.DBConnection.Close();

            return RedirectToPage("../Index");
        }
    }
}
