using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Organizer
{
    public class EditEventModel : PageModel
    {
        public List<Event> Events { get; set; }

        public List<Event> Subevents { get; set; }

        public EditEventModel()
        {
            Events = new List<Event>();
            Subevents = new List<Event>();
        }
        public IActionResult OnGet(int eventID)
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            // query to select the event selected and display all event details in detail
            string sqlQuery = "SELECT Event.*, [Space].*, Location.* FROM Location INNER JOIN " +
                               "[Space] ON Location.LocationID = [Space].LocationID INNER JOIN Event INNER JOIN EventSpace ON Event.EventID = EventSpace.EventID ON[Space].SpaceID = EventSpace.SpaceID " +
                               "WHERE Event.EventID = " + eventID + "AND [Event].OrganizerID = " + HttpContext.Session.GetString("userid") +
                               " ORDER BY Event.StartDate DESC";

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
                    EventType = scheduleReader["Type"].ToString(),
                    Status = scheduleReader["Status"].ToString(),
                    SpaceName = scheduleReader["Name"].ToString(),
                    SpaceAddress = scheduleReader["Address"].ToString()
                });
            }

            DBClass.DBConnection.Close();

            sqlQuery = "SELECT Event.*, [Space].*, Location.* FROM Location INNER JOIN " +
                               "[Space] ON Location.LocationID = [Space].LocationID INNER JOIN Event INNER JOIN EventSpace ON Event.EventID = EventSpace.EventID ON[Space].SpaceID = EventSpace.SpaceID " +
                               "WHERE Event.ParentEventID = " + eventID + "AND [Event].OrganizerID = " + HttpContext.Session.GetString("userid") +
                               " ORDER BY Event.StartDate DESC";

            SqlDataReader subeventReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (scheduleReader.Read())
            {
                Events.Add(new Event
                {
                    EventID = Int32.Parse(subeventReader["EventID"].ToString()),
                    EventName = subeventReader["EventName"].ToString(),
                    EventDescription = subeventReader["EventDescription"].ToString(),
                    StartDate = (DateTime)subeventReader["StartDate"],
                    EndDate = (DateTime)subeventReader["EndDate"],
                    RegistrationDeadline = (DateTime)subeventReader["RegistrationDeadline"],
                    Capacity = Int32.Parse(subeventReader["Capacity"].ToString()),
                    EventType = subeventReader["Type"].ToString(),
                    Status = subeventReader["Status"].ToString(),
                    SpaceName = subeventReader["Name"].ToString(),
                    SpaceAddress = subeventReader["Address"].ToString()
                });
            }

            DBClass.DBConnection.Close();

            return Page();
        }
    }
}
