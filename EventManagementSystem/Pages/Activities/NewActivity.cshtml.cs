using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Activities
{
    public class NewActivityModel : PageModel
    {
        [BindProperty]
        public Activity ActivityToCreate { get; set; }

         public List<SelectListItem> Events { get; set; }

        public List<SelectListItem> Rooms { get; set; }

        public NewActivityModel()
        {
            ActivityToCreate = new Activity();
        }

        // No Activity is needed to get in this method,
        // we are not updating an Activity, only creating a new one
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

            // Populate the EventName select control
            SqlDataReader EventsReader = DBClass.GeneralReaderQuery("SELECT * FROM Event");
            Events = new List<SelectListItem>();
            while (EventsReader.Read())
            {
                Events.Add(new SelectListItem(
                    EventsReader["EventName"].ToString(),
                    EventsReader["EventID"].ToString()));
            }
            DBClass.DBConnection.Close();

            // Populate the RoomNumber select control
            SqlDataReader RoomsReader = DBClass.GeneralReaderQuery("SELECT * FROM Room");
            Rooms = new List<SelectListItem>();
            while (RoomsReader.Read())
            {
                Rooms.Add(new SelectListItem(
                    RoomsReader["RoomNumber"].ToString(),
                    RoomsReader["RoomID"].ToString()));
            }
            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery = "INSERT INTO Activity (ActivityName, ActivityDescription, Date, StartTime, EndTime, Type, [Status], EventID, RoomID) VALUES (" +
                "@ActivityName, @ActivityDescription, @Date, @StartTime, @EndTime, " +
                "@Type, @Status, @EventID, @RoomID)";
            using (SqlConnection connection = new SqlConnection(DBClass.CapstoneDBConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@ActivityName", ActivityToCreate.ActivityName);
                    cmd.Parameters.AddWithValue("@ActivityDescription", ActivityToCreate.ActivityDescription);
                    cmd.Parameters.AddWithValue("@Date", ActivityToCreate.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@StartTime", ActivityToCreate.StartTime.ToString("HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@EndTime", ActivityToCreate.EndTime.ToString("HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@Type", ActivityToCreate.Type);
                    cmd.Parameters.AddWithValue("@Status", ActivityToCreate.Status);
                    cmd.Parameters.AddWithValue("@EventID", ActivityToCreate.EventName);
                    cmd.Parameters.AddWithValue("@RoomID", ActivityToCreate.RoomNumber);

                    cmd.ExecuteNonQuery();
                }
            }

            DBClass.DBConnection.Close();

            return RedirectToPage("AdminActivity");
        }

    }
}
