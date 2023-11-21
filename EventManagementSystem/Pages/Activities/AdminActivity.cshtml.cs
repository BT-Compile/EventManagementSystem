using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Activities
{
    public class AdminActivityModel : PageModel
    {
        public List<Activity> Activities { get; set; }

        public AdminActivityModel()
        {
            Activities = new List<Activity>();
        }

        public IActionResult OnGet()
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

            string sqlQuery = "SELECT Activity.*, [Event].EventName, Building.Name AS SpaceID, Room.RoomNumber " +
                "FROM Activity " +
                "INNER JOIN [Event] ON Activity.EventID = [Event].EventID " +
                "INNER JOIN Building ON [Event].BuildingID = Building.BuildingID " +
                "INNER JOIN Room ON Activity.RoomID = Room.RoomID";
            SqlDataReader activityReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (activityReader.Read())
            {
                // Check if EventID is NULL
                int? eventID = activityReader["EventID"] is DBNull ? (int?)null : (int)activityReader["EventID"];

                Activities.Add(new Activity
                {
                    ActivityID = Int32.Parse(activityReader["ActivityID"].ToString()),
                    ActivityName = activityReader["ActivityName"].ToString(),
                    ActivityDescription = activityReader["ActivityDescription"].ToString(),
                    Date = DateTime.Parse(activityReader["Date"].ToString()),
                    StartTime = TimeOnly.Parse(activityReader["StartTime"].ToString()),
                    EndTime = TimeOnly.Parse(activityReader["EndTime"].ToString()),
                    Type = activityReader["Type"].ToString(),
                    Status = activityReader["Status"].ToString(),
                    EventName = activityReader["EventName"].ToString(),
                    BuildingName = activityReader["SpaceID"].ToString(),
                    RoomNumber = Int32.Parse(activityReader["RoomNumber"].ToString())
                });
            }

            DBClass.DBConnection.Close();

            return Page();
        }
    }
}
