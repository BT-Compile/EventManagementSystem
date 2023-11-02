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

            string sqlQuery = "SELECT Event.EventName, Activity.ActivityName, Activity.Date, Building.Name, Room.RoomNumber " +
                                "FROM Activity INNER JOIN ActivityAttendance ON Activity.ActivityID = ActivityAttendance.ActivityID INNER JOIN " +
                                "Event ON Activity.EventID = Event.EventID INNER JOIN Building ON Event.BuildingID = Building.BuildingID INNER JOIN " +
                                "EventAttendance ON Event.EventID = EventAttendance.EventID INNER JOIN [User] ON ActivityAttendance.UserID = " +
                                "[User].UserID AND EventAttendance.UserID = [User].UserID INNER JOIN Room ON Activity.RoomID = Room.RoomID AND Building.BuildingID = Room.BuildingID " +
                                "WHERE Attendance.UserID = " + userID +
                                " ORDER BY Activity.Date;";

            SqlDataReader scheduleViewer = DBClass.GeneralReaderQuery(sqlQuery);

            while (scheduleViewer.Read())
            {
                UserAssociations.Add(new UserAssociations
                {
                    EventName = scheduleViewer["EventName"].ToString(),
                    ActivityName = scheduleViewer["ActivityName"].ToString(),
                    Date = DateTime.Parse(scheduleViewer["Date"].ToString()),
                    BuildingName = scheduleViewer["BuildingName"].ToString(),
                    RoomNumber = Int32.Parse(scheduleViewer["RoomNumber"].ToString())
                });
            }
        }
    }
}
