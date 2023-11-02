using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Rooms
{
    public class EditRoomModel : PageModel
    {
        [BindProperty]
        public Room RoomToUpdate { get; set; }

        public EditRoomModel()
        {
            RoomToUpdate = new Room();
        }

        public IActionResult OnGet(int roomid)
        {
            if (HttpContext.Session.GetString("usertype") != "Admin" && HttpContext.Session.GetString("usertype") == "Attendee")
            {
                return RedirectToPage("/Attendee/Index");
            }
            else if (HttpContext.Session.GetString("usertype") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            SqlDataReader singleRoom = DBClass.SingleRoomReader(roomid);


            while (singleRoom.Read())
            {
                RoomToUpdate.RoomID = roomid;
                RoomToUpdate.RoomName = singleRoom["RoomName"].ToString();
                RoomToUpdate.RoomDescription = singleRoom["RoomDescription"].ToString();
                RoomToUpdate.Capacity
 = (int)singleRoom["Capacity
"];
                
                // Check if ActivityID is DBNull before casting to int
            }

            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery;
                sqlQuery = "UPDATE Room SET RoomName='" + RoomToUpdate.RoomName
                + "',RoomDescription='" + RoomToUpdate.RoomDescription
                + "',Capacity
='" + RoomToUpdate.Capacity

                + "' WHERE RoomID=" + RoomToUpdate.RoomID;

            DBClass.GeneralQuery(sqlQuery);

            DBClass.DBConnection.Close();

            return RedirectToPage("AdminRoom");
        }
    }
}
