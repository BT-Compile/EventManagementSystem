using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Attendee
{
    public class ViewEventModel : PageModel
    {
        public List<Event> Events { get; set; }

        public List<Event> Subevents { get; set; }

        public List<Event> Subevent1 { get; set; }

        public List<Event> Subevent2 { get; set; }

        public List<Event> Subevent3 { get; set; }

        public List<Event> Subevent4 { get; set; }

        public List<Event> Locations { get; set; }

        public List<Amenity> AmenityList { get; set; }

        public ViewEventModel()
        {
            Events = new List<Event>();
            Subevents = new List<Event>();
            Locations = new List<Event>();
            AmenityList = new List<Amenity>();
            Subevent1 = new List<Event>();
            Subevent2 = new List<Event>();
            Subevent3 = new List<Event>();
            Subevent4 = new List<Event>();
        }
        public IActionResult OnGet(int eventid)
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            HttpContext.Session.SetInt32("tempeventidcool", eventid);

            // query to select the event selected and display all event details in detail
            string sqlQuery = "SELECT Event.*, [Space].*, Location.* FROM Location INNER JOIN " +
                               "[Space] ON Location.LocationID = [Space].LocationID INNER JOIN Event INNER JOIN EventSpace ON Event.EventID = EventSpace.EventID ON[Space].SpaceID = EventSpace.SpaceID " +
                               "WHERE Event.EventID = " + eventid;

            SqlDataReader scheduleReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (scheduleReader.Read())
            {
                Events.Add(new Event
                {
                    EventID = Int32.Parse(scheduleReader["EventID"].ToString()),
                    ParentEventID = scheduleReader["ParentEventID"].ToString(),
                    EventName = scheduleReader["EventName"].ToString(),
                    EventDescription = scheduleReader["EventDescription"].ToString(),
                    StartDate = (DateTime)scheduleReader["StartDate"],
                    EndDate = (DateTime)scheduleReader["EndDate"],
                    RegistrationDeadline = (DateTime)scheduleReader["RegistrationDeadline"],
                    Capacity = Int32.Parse(scheduleReader["Capacity"].ToString()),
                    EventType = scheduleReader["Type"].ToString(),
                    Status = scheduleReader["Status"].ToString(),
                    SpaceName = scheduleReader["Name"].ToString(),
                    SpaceAddress = scheduleReader["Address"].ToString()
                });
            }

            //While ParentEventID IS NOT NULL keep looping to find the parenteventid and event details
            foreach (Event e in Events)
            {
                if (e.ParentEventID != "")
                {
                    sqlQuery = "SELECT * FROM [Event] WHERE EventID = " + e.ParentEventID;
                    SqlDataReader subevent1Reader = DBClass.GeneralReaderQuery(sqlQuery);
                    while (subevent1Reader.Read())
                    {
                        Subevent1.Add(new Event
                        {
                            ParentEventID = subevent1Reader["ParentEventID"].ToString(),
                            EventName = subevent1Reader["EventName"].ToString(),
                        });
                    }
                }
                
            }
            DBClass.DBConnection.Close();

            foreach (Event e in Subevent1)
            {
                if (e.ParentEventID != "")
                {
                    sqlQuery = "SELECT * FROM [Event] WHERE EventID = " + e.ParentEventID;
                    SqlDataReader subevent1Reader = DBClass.GeneralReaderQuery(sqlQuery);
                    while (subevent1Reader.Read())
                    {
                        Subevent2.Add(new Event
                        {
                            ParentEventID = subevent1Reader["ParentEventID"].ToString(),
                            EventName = subevent1Reader["EventName"].ToString(),
                        });
                    }
                }
            }

            DBClass.DBConnection.Close();

            foreach (Event e in Subevent2)
            {
                if (e.ParentEventID != "")
                {
                    sqlQuery = "SELECT * FROM [Event] WHERE EventID = " + e.ParentEventID;
                    SqlDataReader subevent1Reader = DBClass.GeneralReaderQuery(sqlQuery);
                    while (subevent1Reader.Read())
                    {
                        Subevent3.Add(new Event
                        {
                            ParentEventID = subevent1Reader["ParentEventID"].ToString(),
                            EventName = subevent1Reader["EventName"].ToString(),
                        });
                    }
                }
            }

            DBClass.DBConnection.Close();

            foreach (Event e in Subevent3)
            {
                if (e.ParentEventID != "")
                {
                    sqlQuery = "SELECT * FROM [Event] WHERE EventID = " + e.ParentEventID;
                    SqlDataReader subevent1Reader = DBClass.GeneralReaderQuery(sqlQuery);
                    while (subevent1Reader.Read())
                    {
                        Subevent4.Add(new Event
                        {
                            ParentEventID = subevent1Reader["ParentEventID"].ToString(),
                            EventName = subevent1Reader["EventName"].ToString(),
                        });
                    }
                }
            }

            DBClass.DBConnection.Close();

            sqlQuery = "SELECT Event.*, Space.*, Location.*, CASE WHEN Space.ParentSpaceID IS NOT NULL THEN ParentSpace.Name ELSE NULL " +
                       "END AS ParentSpaceName FROM Event INNER JOIN EventSpace ON Event.EventID = EventSpace.EventID INNER JOIN Space ON EventSpace.SpaceID = Space.SpaceID INNER JOIN " +
                       "Location ON Space.LocationID = Location.LocationID LEFT JOIN Space ParentSpace ON Space.ParentSpaceID = ParentSpace.SpaceID " +
                       "WHERE [Event].ParentEventID = " + HttpContext.Session.GetInt32("tempeventidcool") +
                       " ORDER BY Event.StartDate ASC";

            SqlDataReader subeventReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (subeventReader.Read())
            {
                Subevents.Add(new Event
                {
                    EventID = Int32.Parse(subeventReader["EventID"].ToString()),
                    EventName = subeventReader["EventName"].ToString(),
                    EventDescription = subeventReader["EventDescription"].ToString(),
                    StartDate = (DateTime)subeventReader["StartDate"],
                    EndDate = (DateTime)subeventReader["EndDate"],
                    RegistrationDeadline = (DateTime)subeventReader["RegistrationDeadline"],
                    Capacity = Int32.Parse(subeventReader["Capacity"].ToString()),
                    EventType = subeventReader["Type"].ToString(),
                    Status = subeventReader["Status"].ToString(),
                    SpaceName = subeventReader["Name"].ToString(),
                    SpaceAddress = subeventReader["Address"].ToString(),
                    ParentSpaceName = subeventReader["ParentSpaceName"].ToString()
                });
            }

            DBClass.DBConnection.Close();
            return Page();
        }

        public IActionResult OnPostSchedule()
        {
            string sqlQuery = "SELECT Event.*, [Space].*, Location.* FROM Location INNER JOIN " +
                               "[Space] ON Location.LocationID = [Space].LocationID INNER JOIN Event INNER JOIN EventSpace ON Event.EventID = EventSpace.EventID ON[Space].SpaceID = EventSpace.SpaceID " +
                               "WHERE Event.EventID = " + HttpContext.Session.GetInt32("tempeventidcool") +
                               " ORDER BY Event.StartDate ASC";

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
                    SpaceName = scheduleReader["Name"].ToString(),
                    SpaceAddress = scheduleReader["Address"].ToString()
                });
            }

            DBClass.DBConnection.Close();

            sqlQuery = "SELECT Event.*, Space.*, Location.*, CASE WHEN Space.ParentSpaceID IS NOT NULL THEN ParentSpace.Name ELSE NULL " +
                       "END AS ParentSpaceName FROM Event INNER JOIN EventSpace ON Event.EventID = EventSpace.EventID INNER JOIN Space ON EventSpace.SpaceID = Space.SpaceID INNER JOIN " +
                       "Location ON Space.LocationID = Location.LocationID LEFT JOIN Space ParentSpace ON Space.ParentSpaceID = ParentSpace.SpaceID " +
                       "WHERE [Event].ParentEventID = " + HttpContext.Session.GetInt32("tempeventidcool") +
                       " ORDER BY Event.StartDate ASC";

            SqlDataReader subeventReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (subeventReader.Read())
            {
                Subevents.Add(new Event
                {
                    EventID = Int32.Parse(subeventReader["EventID"].ToString()),
                    EventName = subeventReader["EventName"].ToString(),
                    EventDescription = subeventReader["EventDescription"].ToString(),
                    StartDate = (DateTime)subeventReader["StartDate"],
                    EndDate = (DateTime)subeventReader["EndDate"],
                    RegistrationDeadline = (DateTime)subeventReader["RegistrationDeadline"],
                    Capacity = Int32.Parse(subeventReader["Capacity"].ToString()),
                    EventType = subeventReader["Type"].ToString(),
                    Status = subeventReader["Status"].ToString(),
                    SpaceName = subeventReader["Name"].ToString(),
                    SpaceAddress = subeventReader["Address"].ToString(),
                    ParentSpaceName = subeventReader["ParentSpaceName"].ToString()
                });
            }

            DBClass.DBConnection.Close();
            return Page();
        }

        public IActionResult OnPostLocation()
        {
            string sqlQuery = "SELECT Event.*, [Space].*, Location.* FROM Location INNER JOIN " +
                               "[Space] ON Location.LocationID = [Space].LocationID INNER JOIN Event INNER JOIN EventSpace ON Event.EventID = EventSpace.EventID ON[Space].SpaceID = EventSpace.SpaceID " +
                               "WHERE Event.EventID = " + HttpContext.Session.GetInt32("tempeventidcool") +
                               " ORDER BY Event.StartDate ASC";

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
                    SpaceName = scheduleReader["Name"].ToString(),
                    SpaceAddress = scheduleReader["Address"].ToString()
                });
            }
            DBClass.DBConnection.Close();

            sqlQuery = "SELECT Event.*, [Space].*, Location.* FROM Location INNER JOIN " +
                               "[Space] ON Location.LocationID = [Space].LocationID INNER JOIN Event INNER JOIN EventSpace ON Event.EventID = EventSpace.EventID ON[Space].SpaceID = EventSpace.SpaceID " +
                               "WHERE Event.EventID = " + HttpContext.Session.GetInt32("tempeventidcool") +
                               " ORDER BY Event.StartDate ASC";

            SqlDataReader subeventReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (subeventReader.Read())
            {
                Locations.Add(new Event
                {
                    SpaceName = subeventReader["Name"].ToString(),
                    SpaceAddress = subeventReader["Address"].ToString(),
                });
            }

            DBClass.DBConnection.Close();
            return Page();
        }

        public IActionResult OnPostOrganizers()
        {
            string sqlQuery = "SELECT Event.*, [Space].*, Location.* FROM Location INNER JOIN " +
                               "[Space] ON Location.LocationID = [Space].LocationID INNER JOIN Event INNER JOIN EventSpace ON Event.EventID = EventSpace.EventID ON[Space].SpaceID = EventSpace.SpaceID " +
                               "WHERE Event.EventID = " + HttpContext.Session.GetInt32("tempeventidcool") +
                               " ORDER BY Event.StartDate ASC";

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
                    SpaceName = scheduleReader["Name"].ToString(),
                    SpaceAddress = scheduleReader["Address"].ToString()
                });
            }

            DBClass.DBConnection.Close();
            return Page();
        }

        public IActionResult OnPostAmenities()
        {
            string sqlQuery = "SELECT Event.*, [Space].*, Location.* FROM Location INNER JOIN " +
                               "[Space] ON Location.LocationID = [Space].LocationID INNER JOIN Event INNER JOIN EventSpace ON Event.EventID = EventSpace.EventID ON[Space].SpaceID = EventSpace.SpaceID " +
                               "WHERE Event.EventID = " + HttpContext.Session.GetInt32("tempeventidcool") +
                               " ORDER BY Event.StartDate ASC";

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
                    LocationID = Int32.Parse(scheduleReader["LocationID"].ToString()),
                    SpaceName = scheduleReader["Name"].ToString(),
                    SpaceAddress = scheduleReader["Address"].ToString()
                });
            }
            DBClass.DBConnection.Close();

            foreach (Event e in Events)
            {
                sqlQuery = "SELECT * FROM Amenity WHERE LocationID = " + e.LocationID;
                SqlDataReader amenityReader = DBClass.GeneralReaderQuery(sqlQuery);
                while (amenityReader.Read())
                {
                    AmenityList.Add(new Amenity
                    {
                        AmenityID = Int32.Parse(amenityReader["AmenityID"].ToString()),
                        Name = amenityReader["Name"].ToString(),
                        Description = amenityReader["Description"].ToString(),
                        Type = amenityReader["Type"].ToString(),
                        URL = amenityReader["URL"].ToString()
                    });
                }
            }
            DBClass.DBConnection.Close();
            return Page();
        }
    }
}
