using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Attendee.AttendeeSignUp.Cancel
{
    public class CancelMainModel : PageModel
    {
        [BindProperty]
        public Event ParentEvent { get; set; }

        public List<Event> Events { get; set; }

        [BindProperty]
        public List<int> Checked { get; set; }

        public CancelMainModel()
        {
            Events = new List<Event>();
            ParentEvent = new Event();
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
                    EventID = int.Parse(scheduleReader["EventID"].ToString()),
                    EventName = scheduleReader["EventName"].ToString(),
                    EventDescription = scheduleReader["EventDescription"].ToString(),
                    StartDate = (DateTime)scheduleReader["StartDate"],
                    EndDate = (DateTime)scheduleReader["EndDate"],
                    SpaceName = scheduleReader["Name"].ToString()
                });
            }

            DBClass.DBConnection.Close();

            sqlQuery = "SELECT * FROM Event WHERE EventID = " + eventID;
            SqlDataReader singleEvent = DBClass.GeneralReaderQuery(sqlQuery);

            while (singleEvent.Read())
            {
                ParentEvent.EventID = eventID;
                ParentEvent.EventName = singleEvent["EventName"].ToString();
                ParentEvent.StartDate = (DateTime)singleEvent["StartDate"];
                ParentEvent.EndDate = (DateTime)singleEvent["EndDate"];
            }

            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost(List<int> Checked)
        {
            foreach (int i in Checked)
            {
                string sqlQuery = "DELETE FROM EventRegister " +
                "WHERE UserID = " + HttpContext.Session.GetString("userid") +
                " AND EventID = " + i;
                DBClass.GeneralQuery(sqlQuery);
                DBClass.DBConnection.Close();
            }

            return RedirectToPage("../Index");
        }
    }
}
