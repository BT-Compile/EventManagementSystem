using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EventManagementSystem.Pages.Rooms
{
    public class NewRoomModel : PageModel
    {
        [BindProperty]
        public Room RoomToCreate { get; set; }

        public NewRoomModel()
        {
            RoomToCreate = new Room();
        }

        // No Room is needed to get in this method,
        // we are not updating an Room, only creating a new one
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

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery = "INSERT INTO Room (RoomName, RoomDescription, MaxCapacity, IsActive) VALUES (" +
                "'" + RoomToCreate.RoomName + "'," +
                "'" + RoomToCreate.RoomDescription + "'," +
                "'" + RoomToCreate.MaxCapacity + "',1)";

            DBClass.GeneralQuery(sqlQuery);

            DBClass.LabDBConnection.Close();

            return RedirectToPage("AdminRoom");
        }
    }
}
