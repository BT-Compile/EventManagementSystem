using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Locations
{
    public class IndexModel : PageModel
    {
        public List<Location> Locations { get; set; }

        public IndexModel()
        {
            Locations = new List<Location>();
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

            string sqlQuery = "SELECT * FROM Location";
            SqlDataReader locationReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (locationReader.Read())
            {
                Locations.Add(new Location
                {
                    LocationID = Int32.Parse(locationReader["LocationID"].ToString()),
                    City = locationReader["City"].ToString(),
                    State = locationReader["State"].ToString()
                });
            }

            DBClass.DBConnection.Close();

            return Page();
        }
    }
}
