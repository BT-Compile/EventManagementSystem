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

            string sqlQuery = "SELECT * FROM Room";

            SqlDataReader roomReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (roomReader.Read())
            {
                // Check if ActivityID is NULL
                

                Rooms.Add(new Room
                {
                    RoomID = Int32.Parse(roomReader["RoomID"].ToString()),
                    RoomName = roomReader["RoomName"].ToString(),
                    RoomDescription = roomReader["RoomDescription"].ToString(),
                    MaxCapacity = Int32.Parse(roomReader["MaxCapacity"].ToString()),
                    IsActive = (bool)roomReader["IsActive"]
                });
            }

            DBClass.LabDBConnection.Close();

            return Page();
        }
    }
}
