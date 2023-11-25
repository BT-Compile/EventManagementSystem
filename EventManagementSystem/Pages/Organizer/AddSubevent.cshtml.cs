using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Organizer
{
    public class AddSubeventModel : PageModel
    {
        public string Id { get; set; }

        public int? ParentID { get; set; }

        [BindProperty]
        public int? RouteID { get; set; }

        public int ParentLocationID { get; set; }

        public int? ParentLocationIDGet { get; set; }

        public int ParentSpaceID { get; set; }

        public int? ParentSpaceIDGet { get; set; }

        [BindProperty]
        public Event SubeventToCreate { get; set; }

        [BindProperty]
        public Space SpaceToCreate { get; set; }

        public void OnGet(int eventID)
        {
            //Add data to populate room dropdown

            HttpContext.Session.SetInt32("parentid", eventID);

            string sqlQuery = "SELECT [Space].LocationID, [Space].SpaceID FROM  Event INNER JOIN EventSpace ON Event.EventID = EventSpace.EventID INNER JOIN " +
                              "[Space] ON EventSpace.SpaceID = [Space].SpaceID WHERE Event.EventID = " + eventID;

            SqlDataReader singleLocation = DBClass.GeneralReaderQuery(sqlQuery);
            while (singleLocation.Read())
            {
                ParentLocationID = Int32.Parse(singleLocation["LocationID"].ToString());
                ParentSpaceID = Int32.Parse(singleLocation["SpaceID"].ToString());
            }
            DBClass.DBConnection.Close();

            HttpContext.Session.SetInt32("parentlocationid", ParentLocationID);
            HttpContext.Session.SetInt32("parentspaceid", ParentSpaceID);

            RouteID = eventID;
        }

        public IActionResult OnPost()
        {
            Id = HttpContext.Session.GetString("userid");
            ParentID = HttpContext.Session.GetInt32("parentid");
            ParentLocationIDGet = HttpContext.Session.GetInt32("parentlocationid");
            ParentSpaceIDGet = HttpContext.Session.GetInt32("parentspaceid");

            //Creates an event with parameterized entry
            DBClass.SecureSubeventCreation(SubeventToCreate.EventName, SubeventToCreate.EventDescription, SubeventToCreate.StartDate, SubeventToCreate.EndDate, SubeventToCreate.RegistrationDeadline,
                                        SubeventToCreate.Capacity, SubeventToCreate.EventType, Id, ParentID);
            DBClass.DBConnection.Close();

            //Creates space with locationID at ParentSpace
            DBClass.SecureSubeventSpaceCreation(SpaceToCreate.Name, SpaceToCreate.Address, SpaceToCreate.Capacity, ParentLocationIDGet, ParentSpaceIDGet);
            DBClass.DBConnection.Close();

            //Acquire EventID to Create an eventSpace entity
            string sqlQuery = "SELECT EventID From [Event] WHERE EventName = '" + SubeventToCreate.EventName + "' AND OrganizerID = " + Id + " AND ParentEventID = " + ParentID;
            SqlDataReader singleEvent = DBClass.GeneralReaderQuery(sqlQuery);
            while (singleEvent.Read())
            {
                SubeventToCreate.EventID = Int32.Parse(singleEvent["EventID"].ToString());
            }
            DBClass.DBConnection.Close();

            sqlQuery = "SELECT SpaceID From [Space] WHERE Name = '" + SpaceToCreate.Name + "' AND LocationID = " + ParentLocationIDGet + " AND ParentSpaceID = " + ParentSpaceIDGet;
            SqlDataReader singleSpace = DBClass.GeneralReaderQuery(sqlQuery);
            while (singleSpace.Read())
            {
                SpaceToCreate.SpaceID = Int32.Parse(singleSpace["SpaceID"].ToString());
            }
            DBClass.DBConnection.Close();

            //Create EventSpace Entity
            sqlQuery = "INSERT INTO EventSpace (EventID, SpaceID) " +
                       "VALUES (" + SubeventToCreate.EventID + ", " + SpaceToCreate.SpaceID + ")";
            DBClass.GeneralReaderQuery(sqlQuery);
            DBClass.DBConnection.Close();

            int? eventID = ParentID;

            return RedirectToPage("/Organizer/EditEvent", new { eventID });
        }
    }
}
