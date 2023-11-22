using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Locations
{
    public class EditLocationModel : PageModel
    {
        [BindProperty]
        public Location LocationToUpdate { get; set; }

        public EditLocationModel()
        {
            LocationToUpdate = new Location();
        }

        public IActionResult OnGet(int locationid)
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

            SqlDataReader singleLocation = DBClass.SingleLocationReader(locationid);
            if (singleLocation.Read())
            {
                LocationToUpdate.LocationID = locationid;
                LocationToUpdate.City = singleLocation["City"].ToString();
                LocationToUpdate.State = singleLocation["State"].ToString();
            }
            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery = "UPDATE Location SET City = @City, State = @State WHERE LocationID = @LocationID";

            using (SqlConnection connection = new SqlConnection(DBClass.CapstoneDBConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@City", LocationToUpdate.City ?? "");
                    cmd.Parameters.AddWithValue("@State", LocationToUpdate.State ?? "");
                    cmd.Parameters.AddWithValue("@LocationID", LocationToUpdate.LocationID);

                    cmd.ExecuteNonQuery();
                }
            }

            DBClass.DBConnection.Close();

            return RedirectToPage("Index");
        }

    }
}
