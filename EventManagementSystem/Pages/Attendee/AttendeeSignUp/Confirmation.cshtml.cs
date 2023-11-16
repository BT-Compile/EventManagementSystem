using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

//Not Using this page for now, but will implement it later

namespace EventManagementSystem.Pages.Attendee.AttendeeSignUp
{
    public class ConfirmationModel : PageModel
    {
        [BindProperty]
        public User UserToSignUp { get; set; }

        [BindProperty]
        public Event EventToSignUp { get; set; }

        public ConfirmationModel()
        {
            UserToSignUp = new User();
            EventToSignUp = new Event();
        }

        public IActionResult OnGet(int eventid)
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

            sqlQuery = "SELECT * FROM Event WHERE EventID = " + eventid;
            SqlDataReader singleActivity = DBClass.GeneralReaderQuery(sqlQuery);

            while (singleActivity.Read())
            {
                EventToSignUp.EventID = eventid;
                EventToSignUp.EventName = singleActivity["EventName"].ToString();
            }

            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery = "INSERT INTO EventAttendance (EventID, UserID, RegistrationDate) VALUES (" +
                EventToSignUp.EventID + ", " + HttpContext.Session.GetString("userid") + ", GETDATE())";
            DBClass.GeneralQuery(sqlQuery);
            DBClass.DBConnection.Close();

            return RedirectToPage("../Index");
        }
    }
}