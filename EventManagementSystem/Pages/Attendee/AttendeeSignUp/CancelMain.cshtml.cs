using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Attendee.AttendeeSignUp
{
    public class CancelMainModel : PageModel
    {
        public List<Event> Events { get; set; }

        public CancelMainModel()
        {
            Events = new List<Event>();
        }

        public IActionResult OnGet(int eventID)
        {
            string sqlQuery = "SELECT Event.EventID, Event.EventName, Event.EventDescription, Event.StartDate, Event.EndDate, Space.Name " +
                                "FROM Event INNER JOIN EventRegister ON Event.EventID = EventRegister.EventID INNER JOIN [User] ON EventRegister.UserID = [User].UserID INNER JOIN " +
                                "EventSpace ON Event.EventID = EventSpace.EventID INNER JOIN [Space] ON EventSpace.SpaceID = Space.SpaceID " +
                                "WHERE Event.ParentEventID = " + eventID + " AND [User].UserID = " + HttpContext.Session.GetString("userid") +
                                " ORDER BY [Event].StartDate DESC";

            SqlDataReader scheduleReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (scheduleReader.Read())
            {
                Events.Add(new Event
                {
                    EventID = Int32.Parse(scheduleReader["EventID"].ToString()),
                    EventName = scheduleReader["EventName"].ToString(),
                    EventDescription = scheduleReader["EventDescription"].ToString(),
                    StartDate = (DateTime)scheduleReader["StartDate"],
                    EndDate = (DateTime)scheduleReader["EndDate"],
                    SpaceName = scheduleReader["Name"].ToString()
                });
            }

            return Page();
        }
    }
}
