using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Users
{
    public class ViewUserModel : PageModel
    {
        [BindProperty]
        public string? UserName { get; set; }

        public List<UserData> UserData { get; set; }

        public ViewUserModel()
        {
            UserData = new List<UserData>();
        }

        public void OnGet(int userID)
        {
            UserName = DBClass.GetUserName(userID);

            string sqlQuery = "SELECT [User].FirstName, [User].LastName, [User].Email, [User].PhoneNumber, [User].Username, [User].Accomodation, UserEvent.EventName, Activity.ActivityName, Activity.Date, Building.Name, Room.RoomNumber " +
                                "FROM Activity INNER JOIN ActivityAttendance ON Activity.ActivityID = ActivityAttendance.ActivityID INNER JOIN " +
                                "Event ON Activity.EventID = Event.EventID INNER JOIN Building ON Event.BuildingID = Building.BuildingID INNER JOIN " +
                                "EventAttendance ON Event.EventID = EventAttendance.EventID INNER JOIN [User] ON ActivityAttendance.UserID = " +
                                "[User].UserID AND EventAttendance.UserID = [User].UserID INNER JOIN Room ON Activity.RoomID = Room.RoomID AND Building.BuildingID = Room.BuildingID " +
                                "WHERE Attendance.UserID = " + userID +
                                " ORDER BY Activity.Date;";

            SqlDataReader UserViewer = DBClass.GeneralReaderQuery(sqlQuery);

            while (UserViewer.Read())
            {
                Users.UserData.Add(new UserData
                {
                    FirstName = UserViewer["FirstName"].ToString(),
                    LastName = UserViewer["LastName"].ToString(),
                    Email = UserViewer["Email"].ToString(),
                    PhoneNumber = UserViewer["PhoneNumber"].ToString(),
                    Username = UserViewer["Username"].ToString(),
                    Accomodation = UserViewer["Accomodation"].ToString(),
                    EventName = UserViewer["EventName"].ToString(),
                    ActivityName = UserViewer["ActivityName"].ToString(),
                    Date = DateTime.Parse(UserViewer["Date"].ToString()),
                    BuildingName = UserViewer["Name"].ToString(),
                    RoomNumber = int.Parse(UserViewer["RoomNumber"].ToString())
                });
            }
        }
    }
}
