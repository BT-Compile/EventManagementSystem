using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EventManagementSystem.Pages.Organizer
{
    public class CreateEventModel : PageModel
    {
        public string Id { get; set; }
        [BindProperty]
        public Event EventToCreate { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            Id = HttpContext.Session.GetString("userid");

            DBClass.SecurePendingEventCreation(EventToCreate.EventName, EventToCreate.EventDescription, EventToCreate.StartDate, EventToCreate.EndDate, EventToCreate.RegistrationDeadline,
                                        EventToCreate.Capacity, EventToCreate.EventType, Id);
            DBClass.DBConnection.Close();

            return RedirectToPage("Index");
        }
    }
}
