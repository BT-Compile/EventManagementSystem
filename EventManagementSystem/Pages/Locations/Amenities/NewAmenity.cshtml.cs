using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Locations.Amenities
{
    public class NewAmenityModel : PageModel
    {
        [BindProperty]
        public Amenity AmenityToCreate { get; set; }

        public NewAmenityModel()
        {
            AmenityToCreate = new Amenity();
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

            HttpContext.Session.SetInt32("locationid", locationid);;

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery = "INSERT INTO Amenity (Name, Description, Type, URL, LocationID) " +
                              "VALUES (@Name, @Description, @Type, @URL, @LocationID)";

            using (SqlConnection connection = new SqlConnection(DBClass.CapstoneDBConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", AmenityToCreate.Name ?? "");
                    cmd.Parameters.AddWithValue("@Description", AmenityToCreate.Description ?? "");
                    cmd.Parameters.AddWithValue("@Type", AmenityToCreate.Type ?? "");
                    cmd.Parameters.AddWithValue("@URL", AmenityToCreate.URL ?? "");
                    cmd.Parameters.AddWithValue("@LocationID", HttpContext.Session.GetInt32("locationid"));

                    cmd.ExecuteNonQuery();
                }
            }

            DBClass.DBConnection.Close();
            HttpContext.Session.Remove("locationid");

            return RedirectToPage("../Index");
        }

    }
}
