using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.SignUp
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public int UserID { get; set; }

        [BindProperty]
        public int ActivityID { get; set; }

        [BindProperty]
        public string? Message { get; set; }

        public List<SelectListItem> Users { get; set; }

        public List<SelectListItem> Activities { get; set; }

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

            // Populate the User SELECT control
            SqlDataReader UserReader = DBClass.GeneralReaderQuery("SELECT * FROM \"User\"");

            Users = new List<SelectListItem>();

            while (UserReader.Read())
            {
                Users.Add(
                    new SelectListItem(
                        UserReader["FirstName"].ToString() + " " + UserReader["LastName"].ToString(),
                        UserReader["UserID"].ToString()));
            }

            DBClass.DBConnection.Close();

            // Populate the Activity SELECT control
            SqlDataReader ActivityReader = DBClass.GeneralReaderQuery("SELECT * FROM Activity");

            Activities = new List<SelectListItem>();

            while (ActivityReader.Read())
            {
                Activities.Add(
                    new SelectListItem(
                        ActivityReader["ActivityName"].ToString(),
                        ActivityReader["ActivityID"].ToString()));
            }

            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            OnGet();

            // prevents the user from sending a post request if they haven't selected the dropdowns
            if (UserID != 0 && ActivityID != 0)
            {
                if (AlreadySignedUp(UserID, ActivityID))
                {
                    Message = "You have already signed up for this event";
                    return Page();
                }

                // Populate the Activity SELECT control
                SqlDataReader ActivityReader = DBClass.GeneralReaderQuery("SELECT * FROM Activity");

                Activities = new List<SelectListItem>();

                while (ActivityReader.Read())
                {
                    Activities.Add(
                    new SelectListItem(
                    ActivityReader["ActivityName"].ToString(),
                    ActivityReader["ActivityID"].ToString()));
                }

                DBClass.DBConnection.Close();

                return RedirectToPage("Confirmation", new { UserID = UserID, ActivityID = ActivityID });
            }

            return Page();
        }

        private bool AlreadySignedUp(int userid, int activityid)
        {
            string sqlQuery = "SELECT * FROM Attendance WHERE UserID = " + userid + " AND ActivityID = " + activityid;

            // Query Database to see if User has already signed up
            SqlDataReader attendanceReader = DBClass.GeneralReaderQuery(sqlQuery);

            if (attendanceReader.HasRows)
            {
                attendanceReader.Close();
                DBClass.DBConnection.Close();
                return true;
            }

            attendanceReader.Close();
            DBClass.DBConnection.Close();
            return false;
        }

    }
}
