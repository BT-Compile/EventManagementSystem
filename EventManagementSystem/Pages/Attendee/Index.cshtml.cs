using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Data.SqlClient;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System.Text.RegularExpressions;

namespace EventManagementSystem.Pages.Attendee
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public bool HasPosted { get; set; }

        [BindProperty]
        public string? InputString { get; set; }

        public string[]? Keywords { get; set; }

        [BindProperty]
        public string FullName { get; set; }

        [BindProperty]
        public Event CheckIn { get; set; }

        [BindProperty]
        public string TeamName { get; set; }

        public List<Event> Events { get; set; }

        public IndexModel()
        {
            Events = new List<Event>();
            CheckIn = new Event();
            HasPosted = false;
        }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            // Retrive the full name of this logged in User's name
            string nameQuery = "SELECT FirstName, LastName FROM \"User\" WHERE Username = '" + HttpContext.Session.GetString("username") + "'";
            SqlDataReader nameReader = DBClass.GeneralReaderQuery(nameQuery);
            if (nameReader.Read())
            {
                FullName = nameReader["FirstName"].ToString() + " " + nameReader["LastName"].ToString();
            }
            nameReader.Close();

            // Retrieve this participant's team name (if any)
            string teamNameQuery = "SELECT Team.Name AS TeamName " +
                "FROM [User] " +
                "INNER JOIN UserTeam ON [User].UserID = UserTeam.UserID " +
                "INNER JOIN Team ON UserTeam.TeamID = Team.TeamID " +
                "WHERE [User].Username = '" + HttpContext.Session.GetString("username") + "'";
            SqlDataReader teamNameReader = DBClass.GeneralReaderQuery(teamNameQuery);

            if (teamNameReader.Read())
            {
                TeamName = teamNameReader["TeamName"].ToString();
            }
            teamNameReader.Close();

            // This only displays the major EVENTS that contain subevents, the parent events only
            // query to select all events that this user has signed up for already
            string sqlQuery =  "SELECT Event.EventID, Event.EventName, Event.EventDescription, Event.StartDate, Event.EndDate, Space.Name, EventCheckIn.EventID AS EventIDEvent, EventCheckIn.UserID AS UserIDUser " +
                                "FROM Event INNER JOIN EventRegister ON Event.EventID = EventRegister.EventID INNER JOIN [User] ON EventRegister.UserID = [User].UserID INNER JOIN " +
                                "EventSpace ON Event.EventID = EventSpace.EventID INNER JOIN [Space] ON EventSpace.SpaceID = Space.SpaceID " +
                                "LEFT OUTER JOIN EventCheckIn ON Event.EventID = EventCheckIn.EventID AND[User].UserID = EventCheckIn.UserID " +
                                "WHERE Event.ParentEventID IS NULL AND [User].UserID = " + HttpContext.Session.GetString("userid") +
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
                    SpaceID = scheduleReader["Name"].ToString(),
                    EventCheckinEventID = scheduleReader["EventIDEvent"].ToString(),
                    EventCheckinUserID = scheduleReader["UserIDUser"].ToString()

                });
            }
            DBClass.DBConnection.Close();

            return Page();
        }

        //Post request for search functionality
        public IActionResult OnPost()
        {
            HasPosted = true;
            Keywords = Regex.Split(InputString, @"\s+");
            string keyword, sqlQuery;

            for (int i = 0; i < Keywords.Length; i++)
            {
                keyword = Keywords[i];

                // query to do a CASE INSENSITIVE search for a keyword in the Activity table 
                sqlQuery = "SELECT Event.EventID, Event.EventName, Event.EventDescription, Event.StartDate, Event.EndDate, Space.Name " +
                           "FROM Event INNER JOIN EventRegister ON Event.EventID = EventRegister.EventID INNER JOIN [User] ON EventRegister.UserID = [User].UserID INNER JOIN " +
                           "EventSpace ON Event.EventID = EventSpace.EventID INNER JOIN [Space] ON EventSpace.SpaceID = Space.SpaceID " +
                           "WHERE Event.ParentEventID IS NULL AND [User].UserID = " + HttpContext.Session.GetString("userid") + " AND (Event.EventDescription LIKE '%" + keyword + "%' OR Event.EventName LIKE'%" + keyword + "%') " +
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
                            SpaceID = eventReader["Name"].ToString()
                        });
                    }
                }
            }
            DBClass.DBConnection.Close();
            return Page();
        }
    }
}