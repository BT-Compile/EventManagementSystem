using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

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

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery = "INSERT INTO Event (EventName, EventDescription, StartDate, EndDate, RegistrationDeadline, Capacity, Status, BuildingID) " +
                             "VALUES (@EventName, @EventDescription, @StartDate, @EndDate, @RegistrationDeadline, @Capacity, @Status, @BuildingID)";

            using (SqlConnection connection = new SqlConnection(DBClass.CapstoneDBConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@EventName", EventToCreate.EventName);
                    cmd.Parameters.AddWithValue("@EventDescription", EventToCreate.EventDescription);
                    cmd.Parameters.AddWithValue("@StartDate", EventToCreate.StartDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@EndDate", EventToCreate.EndDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@RegistrationDeadline", EventToCreate.RegistrationDeadline.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@Capacity", (int)EventToCreate.Capacity);
                    cmd.Parameters.AddWithValue("@Status", EventToCreate.Status);
                    cmd.Parameters.AddWithValue("@BuildingID", 1); // THIS BUILDING ID NEED TO BE CUSTOMIZEABLE - SR

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToPage("Index");
        }


    }
}
