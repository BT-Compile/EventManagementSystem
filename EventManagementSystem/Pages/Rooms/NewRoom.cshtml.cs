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
        public Space SpaceToCreate { get; set; }

        public List<SelectListItem> ParentSpaces { get; set; }

        public NewRoomModel()
        {
            SpaceToCreate = new Space();
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

            // Populate the ParentSpaceName select control
            SqlDataReader parentSpaceReader = DBClass.GeneralReaderQuery("SELECT * FROM Space");
            ParentSpaces = new List<SelectListItem>();
            while (parentSpaceReader.Read())
            {
                ParentSpaces.Add(new SelectListItem(
                    parentSpaceReader["Name"].ToString(),
                    parentSpaceReader["SpaceID"].ToString()));
            }
            DBClass.DBConnection.Close();

            return Page();
        }
        
        public IActionResult OnPost()
        {
            string sqlQuery = "";
            // Case where a new ParentSpaceName is picked
            if (SpaceToCreate.ParentSpaceID != null)
            {
                sqlQuery = "INSERT INTO [Space] ([Name], [Address], Capacity, ParentSpaceID, LocationID) VALUES (" +
                "'" + SpaceToCreate.Name + "', " +
                "'" + SpaceToCreate.Address + "', " +
                "'" + SpaceToCreate.Capacity + "', " +
                "'" + SpaceToCreate.ParentSpaceID + "', NULL)";
            }
            // Case where no new ParentSpaceName is picked
            else
            {
                sqlQuery = "INSERT INTO [Space] ([Name], [Address], Capacity, ParentSpaceID, LocationID) VALUES (" +
                "'" + SpaceToCreate.Name + "', " +
                "'" + SpaceToCreate.Address + "', " +
                "'" + SpaceToCreate.Capacity + "', NULL, NULL)";
            }

            DBClass.GeneralQuery(sqlQuery);

            DBClass.DBConnection.Close();

            return RedirectToPage("AdminRoom");
        }
    }
}
