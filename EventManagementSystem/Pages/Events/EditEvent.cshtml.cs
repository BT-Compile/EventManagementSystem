using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Events
{
    public class EditEventModel : PageModel
    {
        [BindProperty]
        public Event EventToUpdate { get; set; }

        public List<SelectListItem> Buildings { get; set; }

        public EditEventModel()
        {
            EventToUpdate = new Event();
        }

        public IActionResult OnGet(int eventid)
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

            // Read data from the Event table into each field
            // NOTE: This excludes the "BuildingName" field
            SqlDataReader singleEvent = DBClass.SingleEventReader(eventid);
            while (singleEvent.Read())
            {
                EventToUpdate.EventID = eventid;
                EventToUpdate.EventName = singleEvent["EventName"].ToString();
                EventToUpdate.EventDescription = singleEvent["EventDescription"].ToString();
                EventToUpdate.StartDate = DateTime.Parse(singleEvent["StartDate"].ToString());
                EventToUpdate.EndDate = DateTime.Parse(singleEvent["EndDate"].ToString());
                EventToUpdate.RegistrationDeadline = DateTime.Parse(singleEvent["RegistrationDeadline"].ToString());
                EventToUpdate.Capacity = Int32.Parse(singleEvent["Capacity"].ToString());
                EventToUpdate.Status = singleEvent["Status"].ToString();
            }
            DBClass.DBConnection.Close();

            // Populate the BuildingName select control
            SqlDataReader BuildingsReader = DBClass.GeneralReaderQuery("SELECT * FROM Building");
            Buildings = new List<SelectListItem>();
            while (BuildingsReader.Read())
            {
                Buildings.Add(new SelectListItem(
                    BuildingsReader["Name"].ToString(),
                    BuildingsReader["BuildingID"].ToString()));
            }
            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            // Case that the admin inputted a new building for this event to be in
            if (EventToUpdate.SpaceName != null)
            {
                string sqlQuery = "UPDATE Event SET EventName = @EventName, EventDescription = @EventDescription, " +
                "StartDate = @StartDate, EndDate = @EndDate, RegistrationDeadline = @RegistrationDeadline, " +
                "Capacity = @Capacity, Status = @Status, BuildingID = @BuildingID WHERE EventID = @EventID";

                using (SqlConnection connection = new SqlConnection(DBClass.CapstoneDBConnString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@EventName", EventToUpdate.EventName);
                        cmd.Parameters.AddWithValue("@EventDescription", EventToUpdate.EventDescription);
                        cmd.Parameters.AddWithValue("@StartDate", EventToUpdate.StartDate.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@EndDate", EventToUpdate.EndDate.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@RegistrationDeadline", EventToUpdate.RegistrationDeadline.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@Capacity", (int)EventToUpdate.Capacity);
                        cmd.Parameters.AddWithValue("@Status", EventToUpdate.Status);
                        cmd.Parameters.AddWithValue("@BuildingID", EventToUpdate.SpaceName);
                        cmd.Parameters.AddWithValue("@EventID", EventToUpdate.EventID);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            // Case that the admin has left this event's building field blank
            else
            {
                string sqlQuery = "UPDATE Event SET EventName = @EventName, EventDescription = @EventDescription, " +
                "StartDate = @StartDate, EndDate = @EndDate, RegistrationDeadline = @RegistrationDeadline, " +
                "Capacity = @Capacity, Status = @Status WHERE EventID = @EventID";

                using (SqlConnection connection = new SqlConnection(DBClass.CapstoneDBConnString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@EventName", EventToUpdate.EventName);
                        cmd.Parameters.AddWithValue("@EventDescription", EventToUpdate.EventDescription);
                        cmd.Parameters.AddWithValue("@StartDate", EventToUpdate.StartDate.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@EndDate", EventToUpdate.EndDate.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@RegistrationDeadline", EventToUpdate.RegistrationDeadline.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@Capacity", (int)EventToUpdate.Capacity);
                        cmd.Parameters.AddWithValue("@Status", EventToUpdate.Status);
                        cmd.Parameters.AddWithValue("@EventID", EventToUpdate.EventID);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            
            DBClass.DBConnection.Close();

            return RedirectToPage("AdminEvent");
        }

    }
}
