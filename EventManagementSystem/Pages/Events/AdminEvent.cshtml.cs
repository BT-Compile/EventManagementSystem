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
            if (HttpContext.Session.GetString("usertype") != "Admin" && HttpContext.Session.GetString("usertype") == "Attendee")
            {
                return RedirectToPage("/Attendee/Index");
            }
            else if (HttpContext.Session.GetString("usertype") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            string sqlQuery = "SELECT * FROM [Event]";

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
                    Status = eventReader["Status"].ToString()
                });
            }

            DBClass.DBConnection.Close();

            return Page();
        }
    }
}
