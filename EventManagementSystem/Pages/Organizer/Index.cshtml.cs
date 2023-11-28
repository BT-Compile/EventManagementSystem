using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Organizer
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string FullName { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("RoleType") != "Organizer" &&
                (HttpContext.Session.GetString("RoleType") == "Presenter"
                || HttpContext.Session.GetString("RoleType") == "Judge"
                || HttpContext.Session.GetString("RoleType") == "Participant"))
            {
                return RedirectToPage("/Attendee/Index");
            }
            else if (HttpContext.Session.GetString("RoleType") == "Admin")
            {
                return RedirectToPage("/Admin/Index");
            }
            else if (HttpContext.Session.GetString("RoleType") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            // Retrive the full name of this logged in User's name
            string nameQuery = "SELECT FirstName, LastName FROM \"User\" WHERE Username = '" + HttpContext.Session.GetString("username") + "'";
            SqlDataReader nameReader = DBClass.GeneralReaderQuery(nameQuery);
            if (nameReader.Read())
            {
                FullName = nameReader["FirstName"].ToString() + " " + nameReader["LastName"].ToString();
            }
            nameReader.Close();

            DBClass.DBConnection.Close();

            return Page();
        }
    }
}
