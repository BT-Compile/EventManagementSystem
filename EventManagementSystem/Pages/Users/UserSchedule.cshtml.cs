using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Users
{
    public class UserScheduleModel : PageModel
    {
        [BindProperty]
        public string? UserName { get; set; }

        public List<UserAssociations> UserAssociations { get; set; }

        public UserScheduleModel()
        {
            UserAssociations = new List<UserAssociations>();
        }

        public void OnGet(int userID)
        {
            UserName = DBClass.GetUserName(userID);

            string sqlQuery = "SELECT Event.EventName, Event.EventLocation, Activity.ActivityName, Activity.Date " +
                "FROM Event " +
                "INNER JOIN Activity ON Event.EventID = Activity.EventID " +
                "INNER JOIN Attendance ON Activity.ActivityID = Attendance.ActivityID " +
                "WHERE Attendance.UserID = " + userID +
                " ORDER BY Activity.Date;";

            SqlDataReader scheduleViewer = DBClass.GeneralReaderQuery(sqlQuery);

            while (scheduleViewer.Read())
            {
                UserAssociations.Add(new UserAssociations
                {
                    EventName = scheduleViewer["EventName"].ToString(),
                    EventLocation = scheduleViewer["EventLocation"].ToString(),
                    ActivityName = scheduleViewer["ActivityName"].ToString(),
                    DateAndTime = (DateTime)scheduleViewer["Date"]
                });
            }
        }
    }
}
