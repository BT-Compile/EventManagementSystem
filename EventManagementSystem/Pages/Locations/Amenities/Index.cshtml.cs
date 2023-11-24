using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace EventManagementSystem.Pages.Locations.Amenities
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public bool HasPosted { get; set; }

        [BindProperty]
        public string? InputString { get; set; }

        public string[]? Keywords { get; set; }

        [BindProperty]
        public Location Location { get; set; }

        public List<Amenity> Amenities { get; set; }

        [BindProperty]
        public string? City { get; set; }
        [BindProperty]
        public string? State { get; set; }

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

            HttpContext.Session.SetInt32("locationid", locationid);

            SqlDataReader singleLocation = DBClass.SingleLocationReader(locationid);
            if (singleLocation.Read())
            {
                Location.LocationID = locationid;
                Location.City = singleLocation["City"].ToString();
                Location.State = singleLocation["State"].ToString();
            }
            DBClass.DBConnection.Close();

            HttpContext.Session.SetString("city", Location.City);
            HttpContext.Session.SetString("state", Location.State);

            City = HttpContext.Session.GetString("city");
            State = HttpContext.Session.GetString("state");

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

        public IActionResult OnPost()
        {
            HasPosted = true;
            Keywords = Regex.Split(InputString, @"\s+");
            string keyword, sqlQuery;
            int? locationid = HttpContext.Session.GetInt32("locationid");
            City = HttpContext.Session.GetString("city");
            State = HttpContext.Session.GetString("state");

            for (int i = 0; i < Keywords.Length; i++)
            {
                keyword = Keywords[i];

                // query to do a CASE INSENSITIVE search for a keyword in the Amenity table 
                sqlQuery = "SELECT * FROM Amenity WHERE LocationID = " + locationid + " AND ([Name] LIKE '%" + keyword + "%' OR [Description] LIKE '%" + keyword + "%')";
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
            }
            return Page();
        }
    }
}