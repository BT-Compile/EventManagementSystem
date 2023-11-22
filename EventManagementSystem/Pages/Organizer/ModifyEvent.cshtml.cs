using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Organizer
{
    public class ModifyEventModel : PageModel
    {
        public int? TempEventID { get; set; }

        public int TempLocationID { get; set; }

        public int TempSpaceID { get; set; }

        [BindProperty]
        public Event EventToCreate { get; set; }

        [BindProperty]
        public Space SpaceToCreate { get; set; }

        [BindProperty]
        public Location LocationToCreate { get; set; }

        public void OnGet(int eventid)
        {
            HttpContext.Session.SetInt32("eventid", eventid);

            string sqlQuery = "SELECT [Space].* FROM  Location INNER JOIN [Space] ON Location.LocationID = [Space].LocationID INNER JOIN " +
                              "Event INNER JOIN EventSpace ON Event.EventID = EventSpace.EventID ON[Space].SpaceID = EventSpace.SpaceID WHERE Event.EventID = " + eventid;
            SqlDataReader singleLocation = DBClass.GeneralReaderQuery(sqlQuery);
            while (singleLocation.Read())
            {
                TempLocationID = Int32.Parse(singleLocation["LocationID"].ToString());
                TempSpaceID = Int32.Parse(singleLocation["SpaceID"].ToString());
            }
            DBClass.DBConnection.Close();

            HttpContext.Session.SetInt32("parentlocationid", TempLocationID);
            HttpContext.Session.SetInt32("spaceid", TempSpaceID);
        }
        public IActionResult OnPost()
        {
            TempEventID = HttpContext.Session.GetInt32("eventid");
            int? TempLocationID = HttpContext.Session.GetInt32("parentlocationid");
            int? TempSpaceID = HttpContext.Session.GetInt32("spaceid");

            //Updates event details with parameterized entry
            DBClass.SecureEventModification(EventToCreate.EventName, EventToCreate.EventDescription, EventToCreate.StartDate, EventToCreate.EndDate, EventToCreate.RegistrationDeadline,
                                        EventToCreate.Capacity, EventToCreate.EventType, TempEventID);
            DBClass.DBConnection.Close();

            //Updates location with parameterized entry
            DBClass.SecureLocationModification(LocationToCreate.City, LocationToCreate.State, TempLocationID);
            DBClass.DBConnection.Close();

            //Creates space with locationID
            DBClass.SecureSpaceModification(SpaceToCreate.Name, SpaceToCreate.Address, SpaceToCreate.Capacity, TempLocationID, TempSpaceID);
            DBClass.DBConnection.Close();

            int? eventID = TempEventID;

            return RedirectToPage("/Organizer/EditEvent", new { eventID });
        }
    }
}
