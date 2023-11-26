using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Organizer
{
    public class DeactivateEventModel : PageModel
    {
        [BindProperty]
        public Event EventToDelete { get; set; }

        public DeactivateEventModel()
        {
            EventToDelete = new Event();
        }

        public IActionResult OnGet(int eventid)
        {
            if (HttpContext.Session.GetString("RoleType") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            HttpContext.Session.SetInt32("deleteevent", eventid);

            SqlDataReader singleEvent = DBClass.SingleEventReader(eventid);

            while (singleEvent.Read())
            {
                EventToDelete.EventID = eventid;
                EventToDelete.EventName = singleEvent["EventName"].ToString();
            }

            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery = "UPDATE Event SET [Status] = 'Deactivated' WHERE EventID = " + HttpContext.Session.GetInt32("deleteevent");
            DBClass.GeneralQuery(sqlQuery);
            DBClass.DBConnection.Close();

            sqlQuery = "DELETE FROM EventCreate WHERE EventID = " + HttpContext.Session.GetInt32("deleteevent");
            DBClass.GeneralQuery(sqlQuery);
            DBClass.DBConnection.Close();

            return RedirectToPage("/Organizer/ApprovedEvents");
        }
    }
}
