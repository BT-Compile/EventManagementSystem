using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Events
{
    public class DeleteEventModel : PageModel
    {
        [BindProperty]
        public Event EventToDelete { get; set; }

        public DeleteEventModel()
        {
            EventToDelete = new Event();
        }

        public IActionResult OnGet(int eventid)
        {
            if (HttpContext.Session.GetString("usertype") != "Admin" && HttpContext.Session.GetString("usertype") == "Attendee")
            {
                return RedirectToPage("/Attendee/Index");
            }
            else if (HttpContext.Session.GetString("usertype") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            SqlDataReader singleEvent = DBClass.SingleEventReader(eventid);

            while (singleEvent.Read())
            {
                EventToDelete.EventID = eventid;
            }

            DBClass.LabDBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery = "UPDATE Event SET IsActive = 0 WHERE EventID = " + EventToDelete.EventID;

            DBClass.GeneralQuery(sqlQuery);

            DBClass.LabDBConnection.Close();

            return RedirectToPage("/Events/AdminEvent");
        }
    }
}
