using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Rooms
{
    public class NewRoomModel : PageModel
    {
        [BindProperty]
        public Room RoomToCreate { get; set; }

        public List<SelectListItem> Buildings { get; set; }

        public NewRoomModel()
        {
            RoomToCreate = new Room();
        }

        // No Room is needed to get in this method,
        // we are not updating an Room, only creating a new one
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
            string sqlQuery = "INSERT INTO Room (RoomNumber, Capacity, BuildingID) VALUES (" +
                "'" + RoomToCreate.RoomNumber + "', " +
                "'" + RoomToCreate.Capacity + "', " +
                "'" + RoomToCreate.BuildingName + "')";

            DBClass.GeneralQuery(sqlQuery);

            DBClass.DBConnection.Close();

            return RedirectToPage("AdminRoom");
        }
    }
}
