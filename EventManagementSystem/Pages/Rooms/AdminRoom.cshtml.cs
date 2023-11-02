using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Rooms
{
    public class AdminRoomModel : PageModel
    {
        public List<Room> Rooms { get; set; }

        public AdminRoomModel()
        {
            Rooms = new List<Room>();
        }
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("usertype") != "Admin" && HttpContext.Session.GetString("usertype") == "Attendee")
            {
                return RedirectToPage("/Attendee/Index");
            }
            else if (HttpContext.Session.GetString("usertype") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            string sqlQuery = "SELECT Room.RoomID, Building.Name, Room.RoomNumber, Room.Capacity " +
                              "FROM  Building INNER JOIN Room ON Building.BuildingID " +
                              "= Room.BuildingID ORDER BY Building.Name";

            SqlDataReader roomReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (roomReader.Read())
            {
                //Removed room recommendation functionality for now but will reconfigure and add in sprint 2
                Rooms.Add(new Room
                {
                    RoomID = Int32.Parse(roomReader["RoomID"].ToString()),
                    BuildingName = roomReader["Name"].ToString(),
                    RoomNumber = Int32.Parse(roomReader["RoomNumber"].ToString()),
                    Capacity = Int32.Parse(roomReader["Capacity"].ToString())
                });
            }

            DBClass.DBConnection.Close();

            return Page();
        }
    }
}
