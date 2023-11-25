using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Organizer
{
    public class ModifySubeventModel : PageModel
    {
        [BindProperty]
        public Event EventToUpdate { get; set; }

        public List<SelectListItem> Spaces { get; set; }

        public List<SelectListItem> EventType { get; set; }

        public List<SelectListItem> EventStatus { get; set; }

        [BindProperty]
        public int? eventId { get; set; }

        [BindProperty]
        public int? eventID { get; set; }

        public ModifySubeventModel()
        {
            EventToUpdate = new Event();
        }

        public IActionResult OnGet(int tempeventid)
        {
            if (HttpContext.Session.GetString("RoleType") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            HttpContext.Session.SetInt32("tempeventid", tempeventid);
            eventId = HttpContext.Session.GetInt32("tempeventid");

            // Read data from the Event table into each field
            // NOTE: This excludes the "SpaceID" field
            SqlDataReader singleEvent = DBClass.SingleEventReader(tempeventid);
            if (singleEvent.Read())
            {
                EventToUpdate.EventID = tempeventid;
                EventToUpdate.EventName = singleEvent["EventName"].ToString();
                EventToUpdate.EventDescription = singleEvent["EventDescription"].ToString();
                EventToUpdate.StartDate = DateTime.Parse(singleEvent["StartDate"].ToString());
                EventToUpdate.EndDate = DateTime.Parse(singleEvent["EndDate"].ToString());
                EventToUpdate.RegistrationDeadline = DateTime.Parse(singleEvent["RegistrationDeadline"].ToString());
                EventToUpdate.Capacity = Int32.Parse(singleEvent["Capacity"].ToString());
                EventToUpdate.Status = singleEvent["Status"].ToString();
                EventToUpdate.EventType = singleEvent["EventType"].ToString();
                EventToUpdate.TempParentID = Int32.Parse(singleEvent["ParentEventID"].ToString());
            }
            DBClass.DBConnection.Close();

            HttpContext.Session.SetInt32("tempparentid", EventToUpdate.TempParentID);
            eventID = HttpContext.Session.GetInt32("tempparentid");

            SqlDataReader EventTypeReader = DBClass.GeneralReaderQuery("SELECT DISTINCT Type FROM Event");
            EventType = new List<SelectListItem>();
            while (EventTypeReader.Read())
            {
                EventType.Add(new SelectListItem(
                    EventTypeReader["Type"].ToString(),
                    EventTypeReader["Type"].ToString()));
            }
            DBClass.DBConnection.Close();

            // Populate the Space Name select control
            SqlDataReader SpacesReader = DBClass.GeneralReaderQuery("SELECT * FROM Space");
            Spaces = new List<SelectListItem>();
            while (SpacesReader.Read())
            {
                Spaces.Add(new SelectListItem(
                    SpacesReader["Name"].ToString(),
                    SpacesReader["SpaceID"].ToString()));
            }
            DBClass.DBConnection.Close();

            SqlDataReader EventStatusReader = DBClass.GeneralReaderQuery("SELECT DISTINCT [Status] FROM Event");
            EventStatus = new List<SelectListItem>();
            while (EventStatusReader.Read())
            {
                EventStatus.Add(new SelectListItem(
                    EventStatusReader["Status"].ToString(),
                    EventStatusReader["Status"].ToString()));
            }
            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            eventId = HttpContext.Session.GetInt32("tempeventid");
            eventID = HttpContext.Session.GetInt32("tempparentid");

            DBClass.SecureSubeventModification(EventToUpdate.EventName, EventToUpdate.EventDescription, EventToUpdate.StartDate, EventToUpdate.EndDate, EventToUpdate.RegistrationDeadline,
                                        EventToUpdate.Capacity, EventToUpdate.EventType, eventId);
            DBClass.DBConnection.Close();

            return RedirectToPage("/Organizer/EditEvent", new { eventID });
        }
    }
}
