using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Organizer
{
    public class AssignRoleDeleteModel : PageModel
    {
        [BindProperty]
        public Event EventToFind { get; set; }

        [BindProperty]
        public User UserToFind { get; set; }

        public AssignRoleDeleteModel()
        {
            EventToFind = new Event();
            UserToFind = new User();
        }

        public IActionResult OnGet(int userid, int eventid, int roleid)
        {
            if (HttpContext.Session.GetString("RoleType") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            HttpContext.Session.SetInt32("removeroleevent", eventid);
            HttpContext.Session.SetInt32("removeroleid", roleid);
            HttpContext.Session.SetInt32("removeuserid", userid);

            SqlDataReader singleEvent = DBClass.SingleEventReader(eventid);

            while (singleEvent.Read())
            {
                EventToFind.EventID = eventid;
                EventToFind.EventName = singleEvent["EventName"].ToString();
            }

            DBClass.DBConnection.Close();

            SqlDataReader singleUser = DBClass.SingleUserReader(userid);

            while (singleUser.Read())
            {
                UserToFind.FirstName = singleUser["FirstName"].ToString();
                UserToFind.LastName = singleUser["LastName"].ToString();
            }

            DBClass.DBConnection.Close();

            SqlDataReader singleRole = DBClass.SingleRoleReader(roleid);

            while (singleRole.Read())
            {
                UserToFind.RoleID = Int32.Parse(singleRole["RoleID"].ToString());
                UserToFind.RoleName = singleRole["Name"].ToString();
            }

            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery = "DELETE FROM UserRole WHERE UserID = " + HttpContext.Session.GetInt32("removeuserid") + " AND RoleID = " + HttpContext.Session.GetInt32("removeroleid");
            DBClass.GeneralQuery(sqlQuery);
            DBClass.DBConnection.Close();

            sqlQuery = "UPDATE EventRegister SET RoleID = NULL WHERE EventID = " + HttpContext.Session.GetInt32("removeroleevent") + " AND UserID = " + HttpContext.Session.GetInt32("removeuserid");
            DBClass.GeneralQuery(sqlQuery);
            DBClass.DBConnection.Close();

            return RedirectToPage("/Organizer/RoleAssociations");
        }
    }
}
