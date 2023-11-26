using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Organizer.ParticipantTypes
{
    public class DeleteParticipantTypeModel : PageModel
    {
        public IActionResult OnGet(int roleid)
        {
            if (HttpContext.Session.GetString("RoleType") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            HttpContext.Session.SetInt32("roleid", roleid);

            return Page();
        }

        public IActionResult OnPost()
        {
            //Delete connection that users have with role
            string sqlQuery = "DELETE FROM UserRole WHERE RoleID = " + HttpContext.Session.GetInt32("roleid");
            DBClass.GeneralQuery(sqlQuery);
            DBClass.DBConnection.Close();

            //Delete connection that event has with role
            sqlQuery = "DELETE FROM ParticipantEventRole WHERE RoleID = " + HttpContext.Session.GetInt32("roleid");
            DBClass.GeneralQuery(sqlQuery);
            DBClass.DBConnection.Close();

            //Delete the role from the Role table
            sqlQuery = "DELETE FROM [Role] WHERE RoleID = " + HttpContext.Session.GetInt32("roleid");
            DBClass.GeneralQuery(sqlQuery);
            DBClass.DBConnection.Close();

            return RedirectToPage("/Organizer/ParticipantType");
        }

    }
}
