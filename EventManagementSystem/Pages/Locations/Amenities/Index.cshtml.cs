using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Locations.Amenities
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public Location Location { get; set; }

        public List<Amenity> Amenities { get; set; }

        public IndexModel()
        {
            Location = new Location();
            Amenities = new List<Amenity>();
        }

        public IActionResult OnGet(int locationid)
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

            SqlDataReader singleLocation = DBClass.SingleLocationReader(locationid);
            if (singleLocation.Read())
            {
                Location.LocationID = locationid;
                Location.City = singleLocation["City"].ToString();
                Location.State = singleLocation["State"].ToString();
            }
            DBClass.DBConnection.Close();

            string sqlQuery = "SELECT * FROM Amenity WHERE LocationID = " + locationid;
            SqlDataReader amenityReader = DBClass.GeneralReaderQuery(sqlQuery);
            while (amenityReader.Read())
            {
                Amenities.Add(new Amenity
                {
                    AmenityID = Int32.Parse(amenityReader["AmenityID"].ToString()),
                    Name = amenityReader["Name"].ToString(),
                    Description = amenityReader["Description"].ToString(),
                    Type = amenityReader["Type"].ToString(),
                    URL = amenityReader["URL"].ToString()
                });
            }

            return Page();
        }
    }
}
