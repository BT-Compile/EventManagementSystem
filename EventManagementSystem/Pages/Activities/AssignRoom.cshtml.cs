using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Activities
{
    public class AssignRoomModel : PageModel
    {
        public int ActivityId { get; set; }

        [BindProperty]
        public int RoomId { get; set; }

        public List<SelectListItem> Rooms { get; set; }

        public IActionResult OnGet(int activityid)
        {
            if (HttpContext.Session.GetString("usertype") != "Admin" && HttpContext.Session.GetString("usertype") == "Attendee")
            {
                return RedirectToPage("/Attendee/Index");
            }
            else if (HttpContext.Session.GetString("usertype") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            // Sets the activity for the room to be assigned
            ActivityId = activityid;

            SqlDataReader assignRoom = DBClass.AssignRoomReader(activityid);

            Rooms = new List<SelectListItem>();

            while (assignRoom.Read())
            {
                string roomInfo = assignRoom["RoomName"].ToString() + " - Description: " + assignRoom["RoomDescription"].ToString() + " - Max Capacity: " + assignRoom["MaxCapacity"].ToString();
                Rooms.Add(new SelectListItem(roomInfo, assignRoom["RoomID"].ToString()));
            }

            DBClass.LabDBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery;
            if (RoomId != 0)
            {
                sqlQuery = "UPDATE Activity SET RoomID='" + RoomId
                    + "' WHERE ActivityID=" + ActivityId;

                DBClass.GeneralQuery(sqlQuery);

                DBClass.LabDBConnection.Close();
                return RedirectToPage("AdminActivity");
            }
            else
            {
                // Handle the case where no room is selected
                return Page();
            }
        }
    }
}
