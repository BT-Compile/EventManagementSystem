using EventManagementSystem.Pages.DB;
using EventManagementSystem.Pages.Rooms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Users
{
    public class UserAssociationsModel : PageModel
    {
        [BindProperty]
        public int UserID { get; set; }

        public List<SelectListItem> Users { get; set; }

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

            // Populate the Event SELECT control
            SqlDataReader UserReader = DBClass.GeneralReaderQuery("SELECT * FROM \"User\"");

            Users = new List<SelectListItem>();

            while (UserReader.Read())
            {
                Users.Add(new SelectListItem
                (
                    UserReader["UserName"].ToString(),
                    UserReader["UserID"].ToString()
                ));
            }

            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            if (UserID != 0)
            {
                // Redirect to the Schedule page with the selected EventID
                return RedirectToPage("UserSchedule", new { userid = UserID });
            }
            else
            {
                // Handle the case where no event is selected
                return Page();
            }
        }


        //public List<UserAssociations> userAssociations {  get; set; }

        //public UserAssociationsModel()
        //{
        //    userAssociations = new List<UserAssociations>();
        //}

        //public IActionResult OnGet()
        //{
        //    if (HttpContext.Session.GetString("usertype") != "Admin" && HttpContext.Session.GetString("usertype") == "Attendee")
        //    {
        //        return RedirectToPage("/Attendee/Index");
        //    }
        //    else if (HttpContext.Session.GetString("usertype") == null)
        //    {
        //        return RedirectToPage("/Login/Index");
        //    }

        //    string sqlQuery = "SELECT " +
        //        "\"User\".FirstName, \"User\".LastName, \"User\".IsAdmin, Activity.ActivityName, Event.EventName " +
        //        "FROM \"User\" " +
        //        "LEFT JOIN Attendance ON \"User\".UserID = Attendance.UserID " +
        //        "LEFT JOIN Activity ON Attendance.ActivityID = Activity.ActivityID " +
        //        "LEFT JOIN Event ON Activity.EventID = Event.EventID;";

        //    SqlDataReader associationsReader = DBClass.GeneralReaderQuery(sqlQuery);

        //    while (associationsReader.Read())
        //    {
        //        userAssociations.Add(new UserAssociations
        //        {
        //            FirstName = associationsReader["FirstName"].ToString(),
        //            LastName = associationsReader["LastName"].ToString(),
        //            IsAdmin = associationsReader["IsAdmin"].ToString(),
        //            ActivityName = associationsReader["ActivityName"].ToString(),
        //            EventName = associationsReader["EventName"].ToString()
        //        });
        //    }

        //    associationsReader.Close();

        //    return Page();
        //}
    }
}
