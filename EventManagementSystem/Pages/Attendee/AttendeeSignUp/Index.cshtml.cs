using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Attendee.AttendeeSignUp
{
    public class IndexModel : PageModel
    {
        public List<Event> Events { get; set; }

        public IndexModel()
        {
            Events = new List<Event>();
        }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            // query to select all events that this user has not yet signed up for already
            string sqlQuery = "SELECT Event.EventID, Event.EventName, Event.EventDescription, Event.StartDate, Event.EndDate, Event.RegistrationDeadline, [Space].Name " +
                                "FROM  Event INNER JOIN EventSpace ON Event.EventID = EventSpace.EventID INNER JOIN [Space] ON EventSpace.SpaceID = Space.SpaceID " +
                                "WHERE NOT EXISTS( " +
                                "SELECT " + HttpContext.Session.GetString("userid") +
                                "FROM EventRegister " +
                                "WHERE UserID = " + HttpContext.Session.GetString("userid") + " AND EventRegister.EventID = Event.EventID) " +
                                "AND ParentEventID IS NULL ORDER BY Event.StartDate DESC";

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
                    SpaceName = scheduleReader["Name"].ToString()
                });
            }

            DBClass.DBConnection.Close();

            return Page();
        }
    }
}