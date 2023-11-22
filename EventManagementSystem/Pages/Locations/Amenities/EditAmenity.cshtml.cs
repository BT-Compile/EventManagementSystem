using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Locations.Amenities
{
    public class EditAmenityModel : PageModel
    {
        [BindProperty]
        public Amenity AmenityToUpdate { get; set; }

        public EditAmenityModel()
        {
            AmenityToUpdate = new Amenity();
        }

        public IActionResult OnGet(int amenityid)
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

            SqlDataReader singleAmenity = DBClass.SingleAmenityReader(amenityid);
            if (singleAmenity.Read())
            {
                AmenityToUpdate.AmenityID = amenityid;
                AmenityToUpdate.Name = singleAmenity["Name"].ToString();
                AmenityToUpdate.Description = singleAmenity["Description"].ToString();
                AmenityToUpdate.Type = singleAmenity["Type"].ToString();
                AmenityToUpdate.URL = singleAmenity["URL"].ToString();
            }
            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery = "UPDATE Amenity SET " +
                "Name = @Name, Description = @Description, Type = @Type, URL = @URL " +
                "WHERE AmenityID = @AmenityID";

            if (AmenityToUpdate.Name == null)
            {
                AmenityToUpdate.Name = "";
            }
            if (AmenityToUpdate.Description == null)
            {
                AmenityToUpdate.Description = "";
            }
            if (AmenityToUpdate.Type == null)
            {
                AmenityToUpdate.Type = "";
            }
            if (AmenityToUpdate.URL == null)
            {
                AmenityToUpdate.URL = "";
            }

            using (SqlConnection connection = new SqlConnection(DBClass.CapstoneDBConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", AmenityToUpdate.Name);
                    cmd.Parameters.AddWithValue("@Description", AmenityToUpdate.Description);
                    cmd.Parameters.AddWithValue("@Type", AmenityToUpdate.Type);
                    cmd.Parameters.AddWithValue("@URL", AmenityToUpdate.URL);
                    cmd.Parameters.AddWithValue("@AmenityID", AmenityToUpdate.AmenityID);

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToPage("../Index");
        }

    }
}
