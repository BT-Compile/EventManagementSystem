using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.EventSchedule
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public int EventID { get; set; }

        public List<SelectListItem> Events { get; set; }

        public void OnGet()
        {
            // Populate the Event SELECT control
            SqlDataReader EventReader = DBClass.GeneralReaderQuery("SELECT * FROM Event");

            Events = new List<SelectListItem>();

            while (EventReader.Read())
            {
                Events.Add(new SelectListItem
                (
                    EventReader["EventName"].ToString(),
                    EventReader["EventID"].ToString()
                ));
            }

            DBClass.DBConnection.Close();
        }

        public IActionResult OnPost()
        {
            if (EventID != 0)
            {
                // Redirect to the Schedule page with the selected EventID
                return RedirectToPage("Schedule", new { eventid = EventID });
            }
            else
            {
                // Handle the case where no event is selected
                return Page();
            }
        }
    }
}
