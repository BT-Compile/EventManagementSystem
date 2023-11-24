using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Events
{
    public class AdminViewEventModel : PageModel
    {
        public List<Event> Events { get; set; }

        public AdminViewEventModel()
        {
            Events = new List<Event>();
        }
        public IActionResult OnGet(int eventID)
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            // query to select the event selected and display all event details in detail
            string sqlQuery = "SELECT E.EventID, E.EventName, E.EventDescription, E.StartDate, E.EndDate, E.RegistrationDeadline, E.Capacity, " +
                "E.Type AS EventType, E.Status AS EventStatus, S.Name, S.Address, E.ParentEventID, " +
                "CASE " +
                "WHEN E.ParentEventID IS NOT NULL THEN ParentEvent.EventName ELSE NULL " +
                "END AS ParentEventName " +
                "FROM [Event] E " +
                "INNER JOIN EventSpace ES ON E.EventID = ES.EventID " +
                "INNER JOIN [Space] S ON ES.SpaceID = S.SpaceID " +
                "LEFT JOIN [Event] ParentEvent ON E.ParentEventID = ParentEvent.EventID " +
                "WHERE E.EventID = " + eventID + " ORDER BY E.EventName, E.Status";

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
                    RegistrationDeadline = (DateTime)scheduleReader["RegistrationDeadline"],
                    Capacity = Int32.Parse(scheduleReader["Capacity"].ToString()),
                    EventType = scheduleReader["EventType"].ToString(),
                    Status = scheduleReader["EventStatus"].ToString(),
                    SpaceName = scheduleReader["Name"].ToString(),
                    SpaceAddress = scheduleReader["Address"].ToString(),
                    ParentEventID = scheduleReader["ParentEventID"].ToString(),
                    ParentEventName = scheduleReader["ParentEventName"].ToString()
                });
            }

            DBClass.DBConnection.Close();

            return Page();
        }
    }
}
