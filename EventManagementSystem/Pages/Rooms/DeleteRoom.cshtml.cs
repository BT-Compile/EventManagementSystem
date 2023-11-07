using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Rooms
{
    public class DeleteRoomModel : PageModel
    {
        [BindProperty]
        public Room RoomToDelete { get; set; }

        public DeleteRoomModel()
        {
            RoomToDelete = new Room();
        }

        public IActionResult OnGet(int roomid)
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

            SqlDataReader singleRoom = DBClass.SingleRoomReader(roomid);

            while (singleRoom.Read())
            {
                RoomToDelete.RoomID = roomid;
            }

            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery = "UPDATE Room SET IsActive = 0 WHERE RoomID = " + RoomToDelete.RoomID;

            DBClass.GeneralQuery(sqlQuery);

            DBClass.DBConnection.Close();

            return RedirectToPage("/Rooms/AdminRoom");
        }

    }
}
