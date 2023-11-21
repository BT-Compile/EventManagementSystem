using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System.Text.RegularExpressions;

namespace EventManagementSystem.Pages.Attendee.AttendeeSignUp
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public bool HasPosted { get; set; }

        [BindProperty]
        public string? InputString { get; set; }

        public string[]? Keywords { get; set; }

        public List<Event> Events { get; set; }

        public IndexModel()
        {
            Events = new List<Event>();
            HasPosted = false;
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
                    SpaceID = scheduleReader["Name"].ToString()
                });
            }

            DBClass.DBConnection.Close();

            return Page();
        }

        //Post Request for search functionality for events to sign up for
        public IActionResult OnPost()
        {
            HasPosted = true;
            Keywords = Regex.Split(InputString, @"\s+");
            string keyword, sqlQuery;

            for (int i = 0; i < Keywords.Length; i++)
            {
                keyword = Keywords[i];

                // query to do a CASE INSENSITIVE search for a keyword in the Activity table 
                sqlQuery = "SELECT Event.EventID, Event.EventName, Event.EventDescription, Event.StartDate, Event.EndDate, Event.RegistrationDeadline, [Space].Name " +
                                "FROM  Event INNER JOIN EventSpace ON Event.EventID = EventSpace.EventID INNER JOIN [Space] ON EventSpace.SpaceID = Space.SpaceID " +
                                "WHERE NOT EXISTS( " +
                                "SELECT " + HttpContext.Session.GetString("userid") +
                                "FROM EventRegister " +
                                "WHERE UserID = " + HttpContext.Session.GetString("userid") + " AND EventRegister.EventID = Event.EventID) " +
                                "AND ParentEventID IS NULL AND (Event.EventDescription LIKE '%" + keyword + "%' OR Event.EventName LIKE'%" + keyword + "%') " +
                                "ORDER BY Event.StartDate DESC";

                SqlDataReader eventReader = DBClass.GeneralReaderQuery(sqlQuery);

                while (eventReader.Read())
                {
                    int eventID = Int32.Parse(eventReader["EventID"].ToString());

                    // Check if an activity with the same ID already exists in the list
                    if (!Events.Any(a => a.EventID == eventID))
                    {
                        Events.Add(new Event
                        {
                            EventID = eventID,
                            EventName = eventReader["EventName"].ToString(),
                            EventDescription = eventReader["EventDescription"].ToString(),
                            StartDate = (DateTime)eventReader["StartDate"],
                            EndDate = (DateTime)eventReader["EndDate"],
                            RegistrationDeadline = (DateTime)eventReader["RegistrationDeadline"],
                            SpaceID = eventReader["Name"].ToString()
                        });
                    }
                }
            }

            return Page();
        }
    }
}