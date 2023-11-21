using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Rooms
{
    public class AdminRoomModel : PageModel
    {
        public List<Space> Spaces { get; set; }

        public AdminRoomModel()
        {
            Spaces = new List<Space>();
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

            string sqlQuery = "SELECT S1.SpaceID, S1.Name, S1.Address, S1.Capacity, S1.LocationID, S2.Name AS ParentSpaceName " +
                "FROM Space S1 " +
                "LEFT JOIN Space S2 ON S1.ParentSpaceID = S2.SpaceID";
            SqlDataReader spaceReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (spaceReader.Read())
            {
                Spaces.Add(new Space
                {
                    SpaceID = Int32.Parse(spaceReader["SpaceID"].ToString()),
                    Name = spaceReader["Name"].ToString(),
                    Address = spaceReader["Address"].ToString(),
                    Capacity = Int32.Parse(spaceReader["Capacity"].ToString()),
                    ParentSpaceName = spaceReader["ParentSpaceName"].ToString()
                });
            }

            DBClass.DBConnection.Close();

            return Page();
        }
    }
}
