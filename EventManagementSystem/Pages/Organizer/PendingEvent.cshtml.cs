using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace EventManagementSystem.Pages.Organizer
{
    public class PendingEventModel : PageModel
    {
        [BindProperty]
        public bool HasPosted { get; set; }

        [BindProperty]
        public string? InputString { get; set; }

        public string[]? Keywords { get; set; }

        [BindProperty]
        public string FullName { get; set; }

        [BindProperty]
        public string TeamName { get; set; }

        public List<Event> Events { get; set; }

        public PendingEventModel()
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

            // This only displays the major EVENTS that contain subevents, the parent events only
            // query to select all events that this user has signed up for already
            string sqlQuery = "SELECT * FROM PendingEvent";

            SqlDataReader scheduleReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (scheduleReader.Read())
            {
                Events.Add(new Event
                {
                    EventName = scheduleReader["EventName"].ToString(),
                    EventDescription = scheduleReader["EventDescription"].ToString(),
                    StartDate = (DateTime)scheduleReader["StartDate"],
                    EndDate = (DateTime)scheduleReader["EndDate"],
                    RegistrationDeadline = (DateTime)scheduleReader["RegistrationDeadline"],
                    Capacity = Int32.Parse(scheduleReader["Capacity"].ToString()),
                    EventType = scheduleReader["Type"].ToString()
                });
            }

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
                sqlQuery = "SELECT * FROM PendingEvent " +
                           "WHERE UserID = " + HttpContext.Session.GetString("userid") + " AND (EventDescription LIKE '%" + keyword + "%' OR EventName LIKE'%" + keyword + "%') " +
                           "ORDER BY StartDate DESC";

                SqlDataReader eventReader = DBClass.GeneralReaderQuery(sqlQuery);

                while (eventReader.Read())
                {
                        Events.Add(new Event
                        {
                            EventName = eventReader["EventName"].ToString(),
                            EventDescription = eventReader["EventDescription"].ToString(),
                            StartDate = (DateTime)eventReader["StartDate"],
                            EndDate = (DateTime)eventReader["EndDate"],
                            RegistrationDeadline = (DateTime)eventReader["RegistrationDeadline"],
                            Capacity = Int32.Parse(eventReader["Capacity"].ToString()),
                            EventType = eventReader["Type"].ToString()
                        });
                }
            }

            return Page();

        }
    }
}
