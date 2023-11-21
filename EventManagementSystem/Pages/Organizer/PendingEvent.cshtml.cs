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

            // Displays events created by the organizer that are pending approval
            string sqlQuery = "SELECT * FROM [Event] INNER JOIN EventSpace ON Event.EventID = EventSpace.EventID INNER JOIN [Space] ON EventSpace.SpaceID = [Space].SpaceID INNER JOIN " +
                              "[Location] ON [Space].LocationID = [Location].LocationID WHERE [Status] = 'Pending' AND OrganizerID = " + HttpContext.Session.GetString("userid");

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
                    EventType = scheduleReader["Type"].ToString(),
                    UserID = Int32.Parse(scheduleReader["OrganizerID"].ToString()),
                    SpaceName = scheduleReader["Name"].ToString(),
                    SpaceAddress = scheduleReader["Address"].ToString()
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
                sqlQuery = "SELECT * FROM Event " +
                           "WHERE OrganizerID = " + HttpContext.Session.GetString("userid") + " AND (EventDescription LIKE '%" + keyword + "%' OR EventName LIKE'%" + keyword + "%') " +
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
