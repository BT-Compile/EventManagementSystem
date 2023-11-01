using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EventManagementSystem.Pages.Events
{
    public class NewEventModel : PageModel
    {
        [BindProperty]
        public Event EventToCreate { get; set; }

        public NewEventModel()
        {
            EventToCreate = new Event();
        }

        // No Event is needed to get in this method,
        // we are not updating an Event, only creating a new one
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

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery = "INSERT INTO Event (EventName, EventDescription, EventStartDateAndTime, EventEndDateAndTime, EventLocation, IsActive) VALUES (" +
                "'" + EventToCreate.EventName + "'," +
                "'" + EventToCreate.EventDescription + "'," +
                "'" + EventToCreate.EventStartDateAndTime.ToString("yyyy-MM-dd HH:mm:ss") + "'," +
                "'" + EventToCreate.EventEndDateAndTime.ToString("yyyy-MM-dd HH:mm:ss") + "'," +
                "'" + EventToCreate.EventLocation + "',1)";

            DBClass.GeneralQuery(sqlQuery);

            DBClass.DBConnection.Close();

            return RedirectToPage("Index");
        }

    }
}
