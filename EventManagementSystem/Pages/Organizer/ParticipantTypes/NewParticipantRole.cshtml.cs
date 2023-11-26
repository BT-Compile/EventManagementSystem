using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Organizer.ParticipantTypes
{
    public class NewParticipantRoleModel : PageModel
    {
        public List<SelectListItem> EventNames { get; set; }

        [BindProperty]
        public int RoleID { get; set; }

        [BindProperty]
        public Role RoleToCreate { get; set; }

        public List<Role> Roles { get; set; }

        public NewParticipantRoleModel()
        {
            Roles = new List<Role>();
        }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("RoleType") == null)
            {
                return RedirectToPage("/Login/Index");
            }

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
            string sqlQuery = "INSERT INTO [Role] (Name, Description) VALUES (@Name, @Description)";
            using (SqlConnection connection = new SqlConnection(DBClass.CapstoneDBConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", RoleToCreate.Name ?? "");
                    cmd.Parameters.AddWithValue("@Description", RoleToCreate.Description ?? "");

                    cmd.ExecuteNonQuery();
                }
            }
            DBClass.DBConnection.Close();

            sqlQuery = "SELECT * FROM Role WHERE (Name = '" + RoleToCreate.Name + "' AND Description = '" + RoleToCreate.Description + "')";
            SqlDataReader roleReader = DBClass.GeneralReaderQuery(sqlQuery);
            while (roleReader.Read())
            {
                RoleID = Int32.Parse(roleReader["RoleID"].ToString());
            }
            int tempRoleId = RoleID;

            DBClass.DBConnection.Close();

            sqlQuery = "INSERT INTO ParticipantEventRole (EventID, RoleID) VALUES (@EventID, " + tempRoleId + ")";
            using (SqlConnection connection = new SqlConnection(DBClass.CapstoneDBConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@EventID", RoleToCreate.EventID);

                    cmd.ExecuteNonQuery();
                }
            }
            DBClass.DBConnection.Close();

            return RedirectToPage("/Organizer/ParticipantType");
        }
    }
}
