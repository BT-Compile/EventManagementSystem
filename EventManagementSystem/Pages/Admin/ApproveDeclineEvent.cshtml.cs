using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace EventManagementSystem.Pages.Admin
{
    public class ApproveDeclineEventModel : PageModel
    {
        [BindProperty]
        public bool HasPosted { get; set; }

        [BindProperty]
        public string? InputString { get; set; }

        public string[]? Keywords { get; set; }

        public List<Event> Events { get; set; }

        public ApproveDeclineEventModel()
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
            string sqlQuery = "SELECT Event.*, Space.* FROM Event INNER JOIN EventSpace ON Event.EventID = EventSpace.EventID INNER JOIN " +
                              "Space ON EventSpace.SpaceID = Space.SpaceID WHERE [Status] = 'Pending'";

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
                    UserID = Int32.Parse(scheduleReader["OrganizerID"].ToString()),
                    SpaceName = scheduleReader["Name"].ToString(),
                    SpaceAddress = scheduleReader["Address"].ToString()
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

                // query to do a CASE INSENSITIVE search for a keyword in the Event Table 
                sqlQuery = "SELECT Event.*, Space.* FROM Event INNER JOIN EventSpace ON Event.EventID = EventSpace.EventID INNER JOIN " +
                            "Space ON EventSpace.SpaceID = Space.SpaceID WHERE Event.[Status] = 'Pending' " +
                            "AND (Event.EventDescription LIKE '%" + keyword + "%' OR Event.EventName LIKE '%" + keyword + "%') " +
                            "ORDER BY StartDate DESC";

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
                        UserID = Int32.Parse(scheduleReader["OrganizerID"].ToString()),
                        SpaceName = scheduleReader["Name"].ToString(),
                        SpaceAddress = scheduleReader["Address"].ToString()
                    });
                }
            }
            DBClass.DBConnection.Close();
            return Page();

        }
    }
}
