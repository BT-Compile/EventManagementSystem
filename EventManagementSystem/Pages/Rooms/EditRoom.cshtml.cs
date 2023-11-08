using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Rooms
{
    public class EditRoomModel : PageModel
    {
        [BindProperty]
        public Room RoomToUpdate { get; set; }

        public List<SelectListItem> Buildings { get; set; }

        public EditRoomModel()
        {
            RoomToUpdate = new Room();
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
                RoomToUpdate.RoomID = roomid;
                RoomToUpdate.RoomNumber = Int32.Parse(singleRoom["RoomNumber"].ToString());
                RoomToUpdate.Capacity = Int32.Parse(singleRoom["Capacity"].ToString());
                RoomToUpdate.BuildingID = Int32.Parse(singleRoom["BuildingID"].ToString());
            }
            DBClass.DBConnection.Close();

            // Populate the BuildingName select control
            SqlDataReader BuildingsReader = DBClass.GeneralReaderQuery("SELECT * FROM Building");
            Buildings = new List<SelectListItem>();
            while (BuildingsReader.Read())
            {
                Buildings.Add(new SelectListItem(
                    BuildingsReader["Name"].ToString(),
                    BuildingsReader["BuildingID"].ToString()));
            }
            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery;

            // Case where a new BuildingName is picked
            if (RoomToUpdate.BuildingName != null)
            {
                sqlQuery = "UPDATE Room SET RoomNumber='" + RoomToUpdate.RoomNumber
                + "', Capacity='" + RoomToUpdate.Capacity
                + "', BuildingID='" + RoomToUpdate.BuildingName
                + "' WHERE RoomID=" + RoomToUpdate.RoomID;
            }
            // Case where no new BuildingName is picked
            else
            {
                sqlQuery = "UPDATE Room SET RoomNumber='" + RoomToUpdate.RoomNumber
                + "', Capacity='" + RoomToUpdate.Capacity
                + "' WHERE RoomID=" + RoomToUpdate.RoomID;
            }

            DBClass.GeneralQuery(sqlQuery);

            DBClass.DBConnection.Close();

            return RedirectToPage("AdminRoom");
        }
    }
}
