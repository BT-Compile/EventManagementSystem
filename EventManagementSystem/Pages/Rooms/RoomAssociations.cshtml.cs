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

            string sqlQuery = "SELECT Building.Name, Room.RoomNumber, Activity.ActivityName, Event.EventName " +
                                "FROM Building INNER JOIN Room ON Building.BuildingID = Room.BuildingID INNER JOIN " +
                                "Activity ON Room.RoomID = Activity.RoomID INNER JOIN Event ON Building.BuildingID = " +
                                "Event.BuildingID AND Activity.EventID = Event.EventID ORDER BY Building.Name";

            SqlDataReader associationsReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (associationsReader.Read())
            {
                roomAssociations.Add(new RoomAssociation
                {
                    BuildingName = associationsReader["Name"].ToString(),
                    RoomNumber = Int32.Parse(associationsReader["RoomNumber"].ToString()),
                    ActivityName = associationsReader["ActivityName"].ToString(),
                    EventName = associationsReader["EventName"].ToString()
                });
            }

            associationsReader.Close();

            return Page();
        }
    }
}
