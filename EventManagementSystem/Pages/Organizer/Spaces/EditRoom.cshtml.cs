using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Organizer.Spaces
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
            if (HttpContext.Session.GetString("RoleType") != "Organizer" &&
                (HttpContext.Session.GetString("RoleType") == "Presenter" || HttpContext.Session.GetString("RoleType") == "Judge"
                || HttpContext.Session.GetString("RoleType") == "Participant" || HttpContext.Session.GetString("RoleType") == "Admin"))
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
                sqlQuery = "UPDATE Space SET Name = @Name, Address = @Address, Capacity = @Capacity, ParentSpaceID = @ParentSpaceID WHERE SpaceID = @SpaceID";
                using (SqlConnection connection = new SqlConnection(DBClass.CapstoneDBConnString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@Name", SpaceToUpdate.Name ?? "");
                        cmd.Parameters.AddWithValue("@Address", SpaceToUpdate.Address ?? "");
                        cmd.Parameters.AddWithValue("@Capacity", SpaceToUpdate.Capacity);
                        cmd.Parameters.AddWithValue("@ParentSpaceID", SpaceToUpdate.ParentSpaceID ?? null);
                        cmd.Parameters.AddWithValue("@SpaceID", SpaceToUpdate.SpaceID);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            // Case where no new ParentSpaceName is picked
            else
            {
                sqlQuery = "UPDATE Space SET Name = @Name, Address = @Address, Capacity = @Capacity, ParentSpaceID = NULL WHERE SpaceID = @SpaceID";
                using (SqlConnection connection = new SqlConnection(DBClass.CapstoneDBConnString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@Name", SpaceToUpdate.Name ?? "");
                        cmd.Parameters.AddWithValue("@Address", SpaceToUpdate.Address ?? "");
                        cmd.Parameters.AddWithValue("@Capacity", SpaceToUpdate.Capacity);
                        cmd.Parameters.AddWithValue("@SpaceID", SpaceToUpdate.SpaceID);

                        cmd.ExecuteNonQuery();
                    }
                }
            }

            DBClass.DBConnection.Close();

            return RedirectToPage("AdminRoom");
        }

    }
}
