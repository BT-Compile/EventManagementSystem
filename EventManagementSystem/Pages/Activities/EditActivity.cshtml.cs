using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Activities
{
    public class EditActivityModel : PageModel
    {
        [BindProperty]
        public Activity ActivityToUpdate { get; set; }

        public List<SelectListItem> Events { get; set; }

        public List<SelectListItem> Rooms { get; set; }

        public EditActivityModel() 
        {
            ActivityToUpdate = new Activity();
        }

        public IActionResult OnGet(int activityid)
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

            SqlDataReader singleActivity = DBClass.SingleActivityReader(activityid);

            while (singleActivity.Read())
            {
                ActivityToUpdate.ActivityID = activityid;
                ActivityToUpdate.ActivityName = singleActivity["ActivityName"].ToString();
                ActivityToUpdate.ActivityDescription = singleActivity["ActivityDescription"].ToString();
                ActivityToUpdate.Date = DateTime.Parse(singleActivity["Date"].ToString());
                ActivityToUpdate.StartTime = TimeOnly.Parse(singleActivity["StartTime"].ToString());
                ActivityToUpdate.EndTime = TimeOnly.Parse(singleActivity["EndTime"].ToString());
                ActivityToUpdate.Type = singleActivity["Type"].ToString();
                ActivityToUpdate.Status = singleActivity["Status"].ToString();
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
            string sqlQuery;
            // case that both a new EventID and RoomID are inputted
            if (ActivityToUpdate.EventName != null && ActivityToUpdate.RoomNumber != null)
            {
                sqlQuery = "UPDATE Activity SET ActivityName = @ActivityName, ActivityDescription = @ActivityDescription, " +
                    "Date = @Date, StartTime = @StartTime, EndTime = @EndTime, Type = @Type, Status = @Status, " +
                    "EventID = @EventID, RoomID = @RoomID WHERE ActivityID = @ActivityID";

                using (SqlConnection connection = new SqlConnection(DBClass.CapstoneDBConnString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@ActivityName", ActivityToUpdate.ActivityName);
                        cmd.Parameters.AddWithValue("@ActivityDescription", ActivityToUpdate.ActivityDescription);
                        cmd.Parameters.AddWithValue("@Date", ActivityToUpdate.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@StartTime", ActivityToUpdate.StartTime.ToString("HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@EndTime", ActivityToUpdate.EndTime.ToString("HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@Type", ActivityToUpdate.Type);
                        cmd.Parameters.AddWithValue("@Status", ActivityToUpdate.Status);
                        cmd.Parameters.AddWithValue("@EventID", ActivityToUpdate.EventName);
                        cmd.Parameters.AddWithValue("@RoomID", ActivityToUpdate.RoomNumber);
                        cmd.Parameters.AddWithValue("@ActivityID", ActivityToUpdate.ActivityID);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            // case that only a new EventID has been inputted, but not a roomID
            else if (ActivityToUpdate.EventName != null && ActivityToUpdate.RoomNumber == null)
            {
                sqlQuery = "UPDATE Activity SET ActivityName = @ActivityName, ActivityDescription = @ActivityDescription, " +
                    "Date = @Date, StartTime = @StartTime, EndTime = @EndTime, Type = @Type, Status = @Status, " +
                    "EventID = @EventID WHERE ActivityID = @ActivityID";

                using (SqlConnection connection = new SqlConnection(DBClass.CapstoneDBConnString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@ActivityName", ActivityToUpdate.ActivityName);
                        cmd.Parameters.AddWithValue("@ActivityDescription", ActivityToUpdate.ActivityDescription);
                        cmd.Parameters.AddWithValue("@Date", ActivityToUpdate.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@StartTime", ActivityToUpdate.StartTime.ToString("HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@EndTime", ActivityToUpdate.EndTime.ToString("HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@Type", ActivityToUpdate.Type);
                        cmd.Parameters.AddWithValue("@Status", ActivityToUpdate.Status);
                        cmd.Parameters.AddWithValue("@EventID", ActivityToUpdate.EventName);
                        cmd.Parameters.AddWithValue("@ActivityID", ActivityToUpdate.ActivityID);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            // case that only a new RoomID has been inputted, but not an EventID
            else if (ActivityToUpdate.EventName == null && ActivityToUpdate.RoomNumber != null)
            {
                sqlQuery = "UPDATE Activity SET ActivityName = @ActivityName, ActivityDescription = @ActivityDescription, " +
                    "Date = @Date, StartTime = @StartTime, EndTime = @EndTime, Type = @Type, Status = @Status, " +
                    "RoomID = @RoomID WHERE ActivityID = @ActivityID";

                using (SqlConnection connection = new SqlConnection(DBClass.CapstoneDBConnString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@ActivityName", ActivityToUpdate.ActivityName);
                        cmd.Parameters.AddWithValue("@ActivityDescription", ActivityToUpdate.ActivityDescription);
                        cmd.Parameters.AddWithValue("@Date", ActivityToUpdate.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@StartTime", ActivityToUpdate.StartTime.ToString("HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@EndTime", ActivityToUpdate.EndTime.ToString("HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@Type", ActivityToUpdate.Type);
                        cmd.Parameters.AddWithValue("@Status", ActivityToUpdate.Status);
                        cmd.Parameters.AddWithValue("@RoomID", ActivityToUpdate.RoomNumber);
                        cmd.Parameters.AddWithValue("@ActivityID", ActivityToUpdate.ActivityID);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            // case that neither a roomid or eventid have been inputted
            else
            {
                sqlQuery = "UPDATE Activity SET ActivityName = @ActivityName, ActivityDescription = @ActivityDescription, " +
                    "Date = @Date, StartTime = @StartTime, EndTime = @EndTime, Type = @Type, Status = @Status " +
                    "WHERE ActivityID = @ActivityID";

                using (SqlConnection connection = new SqlConnection(DBClass.CapstoneDBConnString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@ActivityName", ActivityToUpdate.ActivityName);
                        cmd.Parameters.AddWithValue("@ActivityDescription", ActivityToUpdate.ActivityDescription);
                        cmd.Parameters.AddWithValue("@Date", ActivityToUpdate.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@StartTime", ActivityToUpdate.StartTime.ToString("HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@EndTime", ActivityToUpdate.EndTime.ToString("HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@Type", ActivityToUpdate.Type);
                        cmd.Parameters.AddWithValue("@Status", ActivityToUpdate.Status);
                        cmd.Parameters.AddWithValue("@ActivityID", ActivityToUpdate.ActivityID);

                        cmd.ExecuteNonQuery();
                    }
                }
            }

            DBClass.DBConnection.Close();
            
            return RedirectToPage("AdminActivity");
        }

    }
}
