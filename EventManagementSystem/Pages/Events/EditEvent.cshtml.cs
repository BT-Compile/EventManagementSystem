using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Events
{
    public class EditEventModel : PageModel
    {
        [BindProperty]
        public Event EventToUpdate { get; set; }

        public List<SelectListItem> Spaces { get; set; }

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
            // NOTE: This excludes the "SpaceID" field
            SqlDataReader singleEvent = DBClass.SingleEventReader(eventid);
            if (singleEvent.Read())
            {
                EventToUpdate.EventID = eventid;
                EventToUpdate.EventName = singleEvent["EventName"].ToString();
                EventToUpdate.EventDescription = singleEvent["EventDescription"].ToString();
                EventToUpdate.StartDate = DateTime.Parse(singleEvent["StartDate"].ToString());
                EventToUpdate.EndDate = DateTime.Parse(singleEvent["EndDate"].ToString());
                EventToUpdate.RegistrationDeadline = DateTime.Parse(singleEvent["RegistrationDeadline"].ToString());
                EventToUpdate.Capacity = Int32.Parse(singleEvent["Capacity"].ToString());
                EventToUpdate.Status = singleEvent["Status"].ToString();
                EventToUpdate.EventType = singleEvent["EventType"].ToString();
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

            return Page();
        }

        public IActionResult OnPost()
        {
            // Case that the admin inputted a new space for this event to be in
            if (EventToUpdate.SpaceID != null)
            {
                string updateEventSpaceQuery = "UPDATE EventSpace SET SpaceID = @SpaceID WHERE EventID = @EventID";

                using (SqlConnection connection = new SqlConnection(DBClass.CapstoneDBConnString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(updateEventSpaceQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@SpaceID", EventToUpdate.SpaceID);
                        cmd.Parameters.AddWithValue("@EventID", EventToUpdate.EventID);

                        cmd.ExecuteNonQuery();
                        HttpContext.Session.Remove("spaceid");
                    }
                }
            }

            string updateEventQuery = "UPDATE Event SET EventName = @EventName, EventDescription = @EventDescription, " +
                "StartDate = @StartDate, EndDate = @EndDate, RegistrationDeadline = @RegistrationDeadline, " +
                "Capacity = @Capacity, Type = @Type, Status = @Status WHERE EventID = @EventID";

            using (SqlConnection connection = new SqlConnection(DBClass.CapstoneDBConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(updateEventQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@EventName", EventToUpdate.EventName);
                    cmd.Parameters.AddWithValue("@EventDescription", EventToUpdate.EventDescription);
                    cmd.Parameters.AddWithValue("@StartDate", EventToUpdate.StartDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@EndDate", EventToUpdate.EndDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@RegistrationDeadline", EventToUpdate.RegistrationDeadline.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@Capacity", (int)EventToUpdate.Capacity);
                    cmd.Parameters.AddWithValue("@Status", EventToUpdate.Status);
                    cmd.Parameters.AddWithValue("@Type", EventToUpdate.EventType);
                    cmd.Parameters.AddWithValue("@EventID", EventToUpdate.EventID);

                    cmd.ExecuteNonQuery();
                }
            }

            DBClass.DBConnection.Close();

            return RedirectToPage("AdminEvent");
        }

    }
}
