using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Rooms
{
    public class IndexModel : PageModel
    {
        public List<Room> Rooms { get; set; }

        public IndexModel()
        {
            Rooms = new List<Room>();
        }

        public void OnGet()
        {
            string sqlQuery = "SELECT * FROM Room";

            SqlDataReader roomReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (roomReader.Read())
            {
                // Check if ActivityID is NULL

                Rooms.Add(new Room
                {
                    RoomName = roomReader["RoomName"].ToString(),
                    RoomDescription = roomReader["RoomDescription"].ToString(),
                    MaxCapacity = Int32.Parse(roomReader["MaxCapacity"].ToString())
                });
            }

            DBClass.LabDBConnection.Close();
        }
    }
}
