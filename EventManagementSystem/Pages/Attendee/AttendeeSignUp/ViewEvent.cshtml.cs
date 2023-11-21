using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Attendee.AttendeeSignUp
{
    public class ViewEventModel : PageModel
    {
        public List<Event> Events { get; set; }

        public ViewEventModel()
        {
            Events = new List<Event>();
        }
        public IActionResult OnGet(int eventID)
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            // query to select the event selected and display all event details in detail
            string sqlQuery = "SELECT [Event].EventID, [Event].EventName, [Event].EventDescription, [Event].StartDate, [Event].EndDate, Event.RegistrationDeadline, Event.Capacity, Event.Type, Event.Status, " +
                              "[Space].Name, [Space].Address, Amenity.Name AS AmenityName, Amenity.Description, Amenity.Type AS AmenityType, Amenity.URL FROM Location INNER JOIN " +
                              "Amenity ON Location.LocationID = Amenity.LocationID INNER JOIN Space ON Location.LocationID = Space.LocationID INNER JOIN EventRegister INNER JOIN " +
                              "Event ON EventRegister.EventID = Event.EventID INNER JOIN EventSpace ON Event.EventID = EventSpace.EventID ON Space.SpaceID = EventSpace.SpaceID " +
                              "INNER JOIN [User] ON EventRegister.UserID = [User].UserID WHERE Event.EventID = " + eventID + "AND [User].UserID = " + HttpContext.Session.GetString("userid") +
                              " ORDER BY Event.StartDate DESC";

            SqlDataReader scheduleReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (scheduleReader.Read())
            {
                Events.Add(new Event
                {
                    EventID = Int32.Parse(scheduleReader["EventID"].ToString()),
                    EventName = scheduleReader["EventName"].ToString(),
                    EventDescription = scheduleReader["EventDescription"].ToString(),
                    StartDate = (DateTime)scheduleReader["StartDate"],
                    EndDate = (DateTime)scheduleReader["EndDate"],
                    RegistrationDeadline = (DateTime)scheduleReader["RegistrationDeadline"],
                    Capacity = Int32.Parse(scheduleReader["Capacity"].ToString()),
                    EventType = scheduleReader["Type"].ToString(),
                    Status = scheduleReader["Status"].ToString(),
                    SpaceID = scheduleReader["Name"].ToString(),
                    SpaceAddress = scheduleReader["Address"].ToString(),
                    AmenityName = scheduleReader["AmenityName"].ToString(),
                    AmenityDescription = scheduleReader["Description"].ToString(),
                    AmenityType = scheduleReader["AmenityType"].ToString(),
                    URL = scheduleReader["URL"].ToString()
                });
            }

            DBClass.DBConnection.Close();

            return Page();
        }
    }
}
