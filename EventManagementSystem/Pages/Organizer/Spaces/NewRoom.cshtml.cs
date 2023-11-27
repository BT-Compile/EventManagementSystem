using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Organizer.Spaces
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
                sqlQuery = "INSERT INTO [Space] ([Name], [Address], Capacity, CreatorID, ParentSpaceID, LocationID) VALUES " +
                    "(@Name, @Address, @Capacity, " + HttpContext.Session.GetString("userid") + ", @ParentSpaceID, NULL)";
                using (SqlConnection connection = new SqlConnection(DBClass.CapstoneDBConnString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@Name", SpaceToCreate.Name ?? "");
                        cmd.Parameters.AddWithValue("@Address", SpaceToCreate.Address ?? "");
                        cmd.Parameters.AddWithValue("@Capacity", SpaceToCreate.Capacity);
                        cmd.Parameters.AddWithValue("@ParentSpaceID", SpaceToCreate.ParentSpaceID);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            // Case where no new ParentSpaceName is picked
            else
            {
                sqlQuery = "INSERT INTO [Space] ([Name], [Address], Capacity, CreatorID, ParentSpaceID, LocationID) VALUES " +
                    "(@Name, @Address, @Capacity, " + HttpContext.Session.GetString("userid") + ", NULL, NULL)";
                using (SqlConnection connection = new SqlConnection(DBClass.CapstoneDBConnString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@Name", SpaceToCreate.Name ?? "");
                        cmd.Parameters.AddWithValue("@Address", SpaceToCreate.Address ?? "");
                        cmd.Parameters.AddWithValue("@Capacity", SpaceToCreate.Capacity);

                        cmd.ExecuteNonQuery();
                    }
                }
            }

            DBClass.DBConnection.Close();

            return RedirectToPage("Index");
        }

    }
}
