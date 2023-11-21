using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Organizer
{
    public class CreateEventModel : PageModel
    {
        public string Id { get; set; }

        [BindProperty]
        public Event EventToCreate { get; set; }

        [BindProperty]
        public Space SpaceToCreate { get; set; }

        [BindProperty]
        public Location LocationToCreate { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            Id = HttpContext.Session.GetString("userid");

            //Creates an event with parameterized entry
            DBClass.SecurePendingEventCreation(EventToCreate.EventName, EventToCreate.EventDescription, EventToCreate.StartDate, EventToCreate.EndDate, EventToCreate.RegistrationDeadline,
                                        EventToCreate.Capacity, EventToCreate.EventType, Id);
            DBClass.DBConnection.Close();

            //Creates a location with parameterized entry
            DBClass.SecurePendingEventLocationCreation(LocationToCreate.City, LocationToCreate.State);
            DBClass.DBConnection.Close();

            //Acquires newly created locationID
            string sqlQuery = "SELECT LocationID From [Location] WHERE City = '" + LocationToCreate.City + "' AND State = '" + LocationToCreate.State + "'";
            SqlDataReader singleLocation = DBClass.GeneralReaderQuery(sqlQuery);
            while (singleLocation.Read())
            {
                LocationToCreate.LocationID = Int32.Parse(singleLocation["LocationID"].ToString());
            }
            DBClass.DBConnection.Close();

            //Creates space with locationID
            DBClass.SecurePendingEventSpaceCreation(SpaceToCreate.Name, SpaceToCreate.Address, SpaceToCreate.Capacity, LocationToCreate.LocationID);
            DBClass.DBConnection.Close();

            //Acuire EventID to Creat an eventSpace entity
            sqlQuery = "SELECT EventID From [Event] WHERE EventName = '" + EventToCreate.EventName + "' AND OrganizerID = " + Id;
            SqlDataReader singleEvent = DBClass.GeneralReaderQuery(sqlQuery);
            while (singleEvent.Read())
            {
                EventToCreate.EventID = Int32.Parse(singleEvent["EventID"].ToString());
            }
            DBClass.DBConnection.Close();

            sqlQuery = "SELECT SpaceID From [Space] WHERE Name = '" + SpaceToCreate.Name + "' AND LocationID = " + LocationToCreate.LocationID;
            SqlDataReader singleSpace = DBClass.GeneralReaderQuery(sqlQuery);
            while (singleSpace.Read())
            {
                SpaceToCreate.SpaceID = Int32.Parse(singleSpace["SpaceID"].ToString());
            }
            DBClass.DBConnection.Close();

            //Create EventSpace Entity
            sqlQuery = "INSERT INTO EventSpace (EventID, SpaceID) " +
                       "VALUES (" + EventToCreate.EventID + ", " + SpaceToCreate.SpaceID + ")";
            DBClass.GeneralReaderQuery(sqlQuery);
            DBClass.DBConnection.Close();

            return RedirectToPage("Index");
        }
    }
}
