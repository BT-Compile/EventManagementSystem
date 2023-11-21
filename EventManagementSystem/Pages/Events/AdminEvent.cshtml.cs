using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Events
{
    public class AdminEventModel : PageModel
    {
        public List<Event> Events { get; set; }

        public AdminEventModel()
        {
            Events = new List<Event>();
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

            string sqlQuery = "SELECT E.EventID, E.EventName, E.EventDescription, E.StartDate, E.EndDate, E.RegistrationDeadline, E.Capacity, " +
                "E.Type AS EventType, E.Status AS EventStatus," +
                "CASE " +
                "   WHEN E.ParentEventID IS NOT NULL THEN ParentEvent.EventName " +
                "   ELSE NULL " +
                "END AS ParentEventName, S.Name AS SpaceID " +
                "FROM [Event] E " +
                "INNER JOIN EventSpace ES ON E.EventID = ES.EventID " +
                "INNER JOIN [Space] S ON ES.SpaceID = S.SpaceID " +
                "LEFT JOIN [Event] ParentEvent ON E.ParentEventID = ParentEvent.EventID";
            SqlDataReader eventReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (eventReader.Read())
            {
                Events.Add(new Event
                {
                    EventID = Int32.Parse(eventReader["EventID"].ToString()),
                    EventName = eventReader["EventName"].ToString(),
                    EventDescription = eventReader["EventDescription"].ToString(),
                    StartDate = DateTime.Parse(eventReader["StartDate"].ToString()),
                    EndDate = DateTime.Parse(eventReader["EndDate"].ToString()),
                    RegistrationDeadline = DateTime.Parse(eventReader["RegistrationDeadline"].ToString()),
                    Capacity = Int32.Parse(eventReader["Capacity"].ToString()),
                    EventType = eventReader["EventType"].ToString(),
                    Status = eventReader["EventStatus"].ToString(),
                    ParentEventName = eventReader["ParentEventName"].ToString(),
                    SpaceID = eventReader["SpaceID"].ToString()
                });
            }

            DBClass.DBConnection.Close();

            return Page();
        }
    }
}
