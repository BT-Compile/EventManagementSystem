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
            string sqlQuery = "INSERT INTO Event (EventName, EventDescription, StartDate, EndDate, RegistrationDeadline, Capacity, Status) VALUES (" +
                "'" + EventToCreate.EventName + "'," +
                "'" + EventToCreate.EventDescription + "'," +
                "'" + EventToCreate.StartDate.ToString("yyyy-MM-dd") + "'," +
                "'" + EventToCreate.EndDate.ToString("yyyy-MM-dd") + "'," +
                "'" + EventToCreate.RegistrationDeadline.ToString("yyyy-MM-dd") + "'," +
                "'" + EventToCreate.Capacity + "'," +
                "'" + EventToCreate.Status + "')";

            DBClass.GeneralQuery(sqlQuery);

            DBClass.DBConnection.Close();

            return RedirectToPage("Index");
        }

    }
}
