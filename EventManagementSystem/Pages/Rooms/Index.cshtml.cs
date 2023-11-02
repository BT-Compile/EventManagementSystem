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
            string sqlQuery = "SELECT Building.Name, Room.RoomNumber, Room.Capacity " +
                              "FROM  Building INNER JOIN Room ON Building.BuildingID " +
                              "= Room.BuildingID ORDER BY Building.Name";

            SqlDataReader roomReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (roomReader.Read())
            {
                // Check if ActivityID is NULL

                Rooms.Add(new Room
                {
                    BuildingName = roomReader["BuildingName"].ToString(),
                    RoomNumber = Int32.Parse(roomReader["RoomName"].ToString()),
                    Capacity = Int32.Parse(roomReader["Capacity"].ToString())
                });
            }

            DBClass.DBConnection.Close();
        }
    }
}
