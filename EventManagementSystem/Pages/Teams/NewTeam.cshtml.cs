using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using EventManagementSystem.Pages.Login;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Teams
{
    public class NewTeamModel : PageModel
    {
        [BindProperty]
        public Team TeamToCreate { get; set; }

        public NewTeamModel()
        {
            TeamToCreate = new Team();
        }

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

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery = "INSERT INTO Team ([Name], [Description], MaxSize) VALUES " +
                "(@Name, @Description, @MaxSize)";

            using (SqlConnection connection = new SqlConnection(DBClass.CapstoneDBConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", TeamToCreate.Name ?? "");
                    cmd.Parameters.AddWithValue("@Description", TeamToCreate.Description ?? "");
                    cmd.Parameters.AddWithValue("@MaxSize", TeamToCreate.MaxSize);

                    cmd.ExecuteNonQuery();
                }
            }

            DBClass.DBConnection.Close();

            return RedirectToPage("Index");
        }

    }
}
