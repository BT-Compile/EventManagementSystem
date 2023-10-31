using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Rooms
{
    public class RoomAssociationsModel : PageModel
    {
        public List<RoomAssociation> roomAssociations { get; set; }

        public RoomAssociationsModel()
        {
            roomAssociations = new List<RoomAssociation>();
        }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("usertype") != "Admin" && HttpContext.Session.GetString("usertype") == "Attendee")
            {
                return RedirectToPage("/Attendee/Index");
            }
            else if (HttpContext.Session.GetString("usertype") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            string sqlQuery = "SELECT Room.RoomName, Activity.ActivityName, Event.EventName " +
                "FROM Room " +
                "LEFT JOIN Activity ON Room.RoomID = Activity.RoomID " +
                "LEFT JOIN Event ON Activity.EventID = Event.EventID;";

            SqlDataReader associationsReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (associationsReader.Read())
            {
                roomAssociations.Add(new RoomAssociation
                {
                    RoomName = associationsReader["RoomName"].ToString(),
                    ActivityName = associationsReader["ActivityName"].ToString(),
                    EventName = associationsReader["EventName"].ToString()
                });
            }

            associationsReader.Close();

            return Page();
        }
    }
}
