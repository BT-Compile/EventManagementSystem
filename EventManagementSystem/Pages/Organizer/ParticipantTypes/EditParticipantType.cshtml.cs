using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Organizer.ParticipantTypes
{
    public class EditParticipantTypeModel : PageModel
    {
        [BindProperty]
        public Role RoleToUpdate { get; set; }

        public List<SelectListItem> EventNames { get; set; }

        public EditParticipantTypeModel()
        {
            RoleToUpdate = new Role();
        }

        public IActionResult OnGet(int roleid)
        {
            if (HttpContext.Session.GetString("RoleType") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            SqlDataReader singleSpace = DBClass.SingleRoleReader(roleid);
            while (singleSpace.Read())
            {
                RoleToUpdate.RoleID = roleid;
                RoleToUpdate.Name = singleSpace["Name"].ToString();
                RoleToUpdate.Description = singleSpace["Description"].ToString();
            }
            DBClass.DBConnection.Close();

            HttpContext.Session.SetInt32("roleid", roleid);

            SqlDataReader eventRoleReader = DBClass.GeneralReaderQuery("SELECT * FROM [Event] WHERE OrganizerID = " + HttpContext.Session.GetString("userid") + " AND [Status] = 'Active' AND ParentEventID IS NULL");
            EventNames = new List<SelectListItem>();
            while (eventRoleReader.Read())
            {
                EventNames.Add(new SelectListItem(
                    eventRoleReader["EventName"].ToString(),
                    eventRoleReader["EventID"].ToString()));
            }
            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery = "UPDATE [Role] SET Name = @Name, Description = @Description";
            using (SqlConnection connection = new SqlConnection(DBClass.CapstoneDBConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                        cmd.Parameters.AddWithValue("@Name", RoleToUpdate.Name ?? "");
                        cmd.Parameters.AddWithValue("@Description", RoleToUpdate.Description ?? "");

                        cmd.ExecuteNonQuery();
                    }
                }
            DBClass.DBConnection.Close();

            sqlQuery = "UPDATE ParticipantEventRole SET EventID = @EventID, RoleID = @RoleID";
            using (SqlConnection connection = new SqlConnection(DBClass.CapstoneDBConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@EventID", RoleToUpdate.EventID);
                    cmd.Parameters.AddWithValue("@RoleID", RoleToUpdate.RoleID);

                    cmd.ExecuteNonQuery();
                }
            }
            DBClass.DBConnection.Close();

            return RedirectToPage("/Organizer/ParticipantType");
        }

    }
}

