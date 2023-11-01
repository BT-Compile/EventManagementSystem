using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Events
{
    public class EditEventModel : PageModel
    {
        [BindProperty]
        public Event EventToUpdate { get; set; }

        public EditEventModel()
        {
            EventToUpdate = new Event();
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
                EventToUpdate.EventID = eventid;
                EventToUpdate.EventName = singleEvent["EventName"].ToString();
                EventToUpdate.EventDescription = singleEvent["EventDescription"].ToString();
                EventToUpdate.EventStartDateAndTime = (DateTime)singleEvent["EventStartDateAndTime"];
                EventToUpdate.EventEndDateAndTime = (DateTime)singleEvent["EventEndDateAndTime"];
                EventToUpdate.EventLocation = singleEvent["EventLocation"].ToString();
            }

            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery = "UPDATE Event SET EventName='" + EventToUpdate.EventName
                + "',EventDescription='" + EventToUpdate.EventDescription
                + "',EventStartDateAndTime='" + EventToUpdate.EventStartDateAndTime
                + "',EventEndDateAndTime='" + EventToUpdate.EventEndDateAndTime
                + "',EventLocation='" + EventToUpdate.EventLocation
                + "' WHERE EventID=" + EventToUpdate.EventID;

            DBClass.GeneralQuery(sqlQuery);

            DBClass.DBConnection.Close();

            return RedirectToPage("AdminEvent");
        }

    }
}
