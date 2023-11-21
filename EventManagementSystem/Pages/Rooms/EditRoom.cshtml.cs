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
        public Space SpaceToUpdate { get; set; }

        public List<SelectListItem> ParentSpaces { get; set; }

        public EditRoomModel()
        {
            SpaceToUpdate = new Space();
        }

        public IActionResult OnGet(int spaceid)
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

            SqlDataReader singleSpace = DBClass.SingleSpaceReader(spaceid);
            while (singleSpace.Read())
            {
                SpaceToUpdate.SpaceID = spaceid;
                SpaceToUpdate.Name = singleSpace["Name"].ToString();
                SpaceToUpdate.Address = singleSpace["Address"].ToString();
                SpaceToUpdate.Capacity = Int32.Parse(singleSpace["Capacity"].ToString());
            }
            DBClass.DBConnection.Close();

            // Populate the ParentSpaceName select control
            SqlDataReader parentSpaceReader = DBClass.GeneralReaderQuery("SELECT * FROM Space WHERE SpaceID <> " + spaceid);
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
            string sqlQuery;

            // Case where a new ParentSpaceName is picked
            if (SpaceToUpdate.ParentSpaceID != null)
            {
                sqlQuery = "UPDATE Space SET Name ='" + SpaceToUpdate.Name
                    + "', Address='" + SpaceToUpdate.Address
                    + "', Capacity='" + SpaceToUpdate.Capacity
                    + "', ParentSpaceID='" + SpaceToUpdate.ParentSpaceID
                    + "' WHERE SpaceID=" + SpaceToUpdate.SpaceID;
            }
            // Case where no new ParentSpaceName is picked
            else
            {
                sqlQuery = "UPDATE Space SET Name='" + SpaceToUpdate.Name
                    + "', Address='" + SpaceToUpdate.Address
                    + "', Capacity='" + SpaceToUpdate.Capacity
                    + "', ParentSpaceID=NULL"
                    + " WHERE SpaceID=" + SpaceToUpdate.SpaceID;
            }

            DBClass.GeneralQuery(sqlQuery);

            DBClass.DBConnection.Close();

            return RedirectToPage("AdminRoom");
        }
    }
}
