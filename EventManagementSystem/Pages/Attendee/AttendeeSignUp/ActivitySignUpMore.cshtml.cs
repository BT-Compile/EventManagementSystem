using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace EventManagementSystem.Pages.Attendee.AttendeeSignUp
{
    public class ActivitySignUpMoreModel : PageModel
    {
        [BindProperty]
        public bool HasPosted { get; set; }

        [BindProperty]
        public string? InputString { get; set; }

        public string[]? Keywords { get; set; }

        public List<Event> Events { get; set; }

        [BindProperty]
        public List<int> Checked { get; set; }

        [BindProperty]
        public Event ParentEvent { get; set; }

        public ActivitySignUpMoreModel()
        {
            Events = new List<Event>();
            ParentEvent = new Event();
            HasPosted = false;
        }

        public IActionResult OnGet(int eventid)
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            HttpContext.Session.SetInt32("EventInt", eventid);

            // query to select all SUBEVENTS within the EVENT selected on the previous page
            string sqlQuery = "SELECT Event.EventID, Event.EventName, Event.EventDescription, Event.StartDate, Event.EndDate, Event.RegistrationDeadline, Space.Name " +
                                "FROM  Event INNER JOIN EventSpace ON Event.EventID = EventSpace.EventID INNER JOIN Space ON EventSpace.SpaceID = Space.SpaceID " +
                                "WHERE NOT EXISTS( " +
                                "SELECT " + HttpContext.Session.GetString("userid") +
                                "FROM EventRegister " +
                                "WHERE UserID = " + HttpContext.Session.GetString("userid") + " AND EventRegister.EventID = Event.EventID) " +
                                "AND ParentEventID = " + eventid + " ORDER BY Event.StartDate DESC";

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

            sqlQuery = "SELECT * FROM Event WHERE EventID = " + eventid;
            SqlDataReader singleActivity = DBClass.GeneralReaderQuery(sqlQuery);

            while (singleActivity.Read())
            {
                ParentEvent.EventID = eventid;
                ParentEvent.EventName = singleActivity["EventName"].ToString();
            }

            DBClass.DBConnection.Close();

            return Page();
        }

        //Post Request for the search bar, returns Activities from search keywords
        public IActionResult OnPostSearch()
        {
            HasPosted = true;
            Keywords = Regex.Split(InputString, @"\s+");
            string keyword, sqlQuery;

            for (int i = 0; i < Keywords.Length; i++)
            {
                keyword = Keywords[i];

                // query to do a CASE INSENSITIVE search for a keyword in the Activity table
                sqlQuery = "SELECT Event.EventID, Event.EventName, Event.EventDescription, Event.StartDate, Event.EndDate, Event.RegistrationDeadline, Space.Name " +
                                "FROM  Event INNER JOIN EventSpace ON Event.EventID = EventSpace.EventID INNER JOIN Space ON EventSpace.SpaceID = Space.SpaceID " +
                                "WHERE NOT EXISTS( " +
                                "SELECT " + HttpContext.Session.GetString("userid") +
                                "FROM EventRegister " +
                                "WHERE UserID = " + HttpContext.Session.GetString("userid") + " AND EventRegister.EventID = Event.EventID) " +
                                "AND ParentEventID = " + HttpContext.Session.GetInt32("EventInt") + " AND (Event.EventDescription LIKE '%" + keyword + "%' OR Event.EventName LIKE'%" + keyword + "%')" +
                                "ORDER BY Event.StartDate DESC";

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

                sqlQuery = "SELECT * FROM Event WHERE EventID = " + HttpContext.Session.GetInt32("EventInt");
                SqlDataReader singleActivity = DBClass.GeneralReaderQuery(sqlQuery);

                while (singleActivity.Read())
                {
                    ParentEvent.EventID = (Int32)HttpContext.Session.GetInt32("EventInt");
                    ParentEvent.EventName = singleActivity["EventName"].ToString();
                }

                DBClass.DBConnection.Close();
            }

            return Page();
        }

        //Post request to register a user for selected events
        public IActionResult OnPostRegister(List<int> Checked)
        {
            foreach (int i in Checked)
            {
                string sqlQuery = "INSERT INTO EventRegister (EventID, UserID, RegistrationDate) VALUES (" +
                i + ", " + HttpContext.Session.GetString("userid") + ", GETDATE())";
                DBClass.GeneralQuery(sqlQuery);
                DBClass.DBConnection.Close();
            }

            int? eventid = HttpContext.Session.GetInt32("EventInt");

            return RedirectToPage("/Attendee/AttendeeSignup/Schedule", new { eventid });
        }
    }
}