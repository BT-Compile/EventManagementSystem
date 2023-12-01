using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Events
{
    public class NewEventModel : PageModel
    {
        [BindProperty]
        public Event EventToCreate { get; set; }

        public List<SelectListItem> Spaces { get; set; }

        public List<SelectListItem> EventType { get; set; }

        public NewEventModel()
        {
            EventToCreate = new Event();
        }

        // No Event is needed to get in this method,
        // we are not updating an Event, only creating a new one
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

            SqlDataReader EventTypeReader = DBClass.GeneralReaderQuery("SELECT DISTINCT Type FROM Event");
            EventType = new List<SelectListItem>();
            while (EventTypeReader.Read())
            {
                EventType.Add(new SelectListItem(
                    EventTypeReader["Type"].ToString(),
                    EventTypeReader["Type"].ToString()));
            }
            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery = "INSERT INTO [Event] (EventName, EventDescription, StartDate, EndDate, RegistrationDeadline, Capacity, [Type], [Status], ParentEventID)" + 
                              "VALUES(@EventName, @EventDescription, @StartDate, @EndDate, @RegistrationDeadline, @Capacity, @[Type], @[Status], @ParentEventID)";

            using (SqlConnection connection = new SqlConnection(DBClass.CapstoneDBConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@EventName", EventToCreate.EventName);
                    cmd.Parameters.AddWithValue("@EventDescription", EventToCreate.EventDescription);
                    cmd.Parameters.AddWithValue("@StartDate", EventToCreate.StartDate.ToString("yyyy-MM-dd hh:mm:ss"));
                    cmd.Parameters.AddWithValue("@EndDate", EventToCreate.EndDate.ToString("yyyy-MM-dd hh:mm:ss"));
                    cmd.Parameters.AddWithValue("@RegistrationDeadline", EventToCreate.RegistrationDeadline.ToString("yyyy-MM-dd hh:mm:ss"));
                    cmd.Parameters.AddWithValue("@Capacity", (int)EventToCreate.Capacity);
                    cmd.Parameters.AddWithValue("@[Type]", EventToCreate.EventType);
                    cmd.Parameters.AddWithValue("@[Status]", EventToCreate.Status);
                    cmd.Parameters.AddWithValue("@ParentEventID", EventToCreate.ParentEventID);

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToPage("/Events/AdminEvent");
        }


    }
}
