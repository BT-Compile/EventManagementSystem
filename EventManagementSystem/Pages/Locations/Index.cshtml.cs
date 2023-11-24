using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System.Text.RegularExpressions;

namespace EventManagementSystem.Pages.Locations
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public bool HasPosted { get; set; }

        [BindProperty]
        public string? InputString { get; set; }

        public string[]? Keywords { get; set; }

        public List<Location> Locations { get; set; }

        public IndexModel()
        {
            Locations = new List<Location>();
            HasPosted = false;
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

            string sqlQuery = "SELECT * FROM Location";
            SqlDataReader locationReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (locationReader.Read())
            {
                Locations.Add(new Location
                {
                    LocationID = Int32.Parse(locationReader["LocationID"].ToString()),
                    City = locationReader["City"].ToString(),
                    State = locationReader["State"].ToString()
                });
            }

            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            HasPosted = true;
            Keywords = Regex.Split(InputString, @"\s+");
            string keyword, sqlQuery;

            for (int i = 0; i < Keywords.Length; i++)
            {
                keyword = Keywords[i];

                // query to do a CASE INSENSITIVE search for a keyword in the Location table 
                sqlQuery = "SELECT * FROM Location WHERE ([City] LIKE '%" + keyword + "%' OR [State] LIKE'%" + keyword + "%')";
                SqlDataReader locationReader = DBClass.GeneralReaderQuery(sqlQuery);

                while (locationReader.Read())
                {
                    Locations.Add(new Location
                    {
                        LocationID = Int32.Parse(locationReader["LocationID"].ToString()),
                        City = locationReader["City"].ToString(),
                        State = locationReader["State"].ToString()
                    });
                }
            }
            return Page();
        }
    }
}
