using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Organizer
{
    public class AssignRoleModel : PageModel
    {
        [BindProperty]
        public EventUser FieldToUpdate { get; set; }

        public List<SelectListItem> UserRole { get; set; }

        public List<SelectListItem> Subevent { get; set; }

        [BindProperty]
        public Event EventNames { get; set; }

        public AssignRoleModel()
        {
            EventNames = new Event();
            FieldToUpdate = new EventUser();
        }

        public IActionResult OnGet(int userid, int eventid)
        {
            if (HttpContext.Session.GetString("RoleType") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            HttpContext.Session.SetInt32("assigneventid", eventid);
            HttpContext.Session.SetInt32("assignuserid", userid);

            string sqlQuery = "SELECT Event.EventID, Event.EventName, Event.EventDescription, Event.StartDate, Event.EndDate, Event.RegistrationDeadline, Event.Capacity, Event.Type, " +
                "Event.Status, Event.OrganizerID, Event.ParentEventID, [User].UserID, concat_ws(' ', [User].FirstName, [User].LastName) AS FullName, Role.RoleID, Role.Name, Role.Description " +
                "FROM  UserRole INNER JOIN Role ON UserRole.RoleID = Role.RoleID INNER JOIN Event LEFT OUTER JOIN EventRegister ON Event.EventID = EventRegister.EventID INNER JOIN " +
                "[User] ON EventRegister.UserID = [User].UserID ON UserRole.UserID = [User].UserID " +
                "WHERE Event.OrganizerID = " + HttpContext.Session.GetString("userid") +
                " AND ([Event].EventID = " + eventid + ") AND [User].IsActive = 'true' AND [User].UserID = " + userid + " ORDER BY EventRegister.RegistrationDate, [Event].EventName, Role.Name DESC";

            SqlDataReader userReader = DBClass.GeneralReaderQuery(sqlQuery);

            if (userReader.Read())
            {
                FieldToUpdate.FirstName = userReader["FullName"].ToString();
                FieldToUpdate.RoleID = (Int32)userReader["RoleID"];
                FieldToUpdate.EventID = (Int32)userReader["EventID"];
            }
            DBClass.DBConnection.Close();

            // Populate the Role Dropdown
            SqlDataReader EventTypeReader = DBClass.GeneralReaderQuery("SELECT DISTINCT [Name], RoleID FROM [Role] WHERE [Name] != 'Admin' AND [Name] != 'Organizer' AND [Name] != 'Participant'");
            UserRole = new List<SelectListItem>();
            while (EventTypeReader.Read())
            {
                UserRole.Add(new SelectListItem(
                    EventTypeReader["Name"].ToString(),
                    EventTypeReader["RoleID"].ToString()));
            }
            DBClass.DBConnection.Close();

            // Populate the Subevent Name select control
            SqlDataReader SpacesReader = DBClass.GeneralReaderQuery("SELECT * FROM Event WHERE ParentEventID = " + eventid);
            Subevent = new List<SelectListItem>();
            while (SpacesReader.Read())
            {
                Subevent.Add(new SelectListItem(
                    SpacesReader["EventName"].ToString(),
                    SpacesReader["EventID"].ToString()));
            }
            DBClass.DBConnection.Close();

            return Page();
        }
        public IActionResult OnPost()
        {
            // MUST SETUP A control flow to not allow duplicate fields to be entered or else the primary key relationship breaks and the program crashes
            //
            string sqlQuery;

            sqlQuery = "INSERT INTO UserRole (UserID, RoleID, AssignDate) VALUES (" + HttpContext.Session.GetInt32("assignuserid") + ", " + FieldToUpdate.RoleID + ", GETDATE())";
            DBClass.GeneralQuery(sqlQuery);

            DBClass.DBConnection.Close();

            sqlQuery = "UPDATE EventRegister SET RoleID = " + FieldToUpdate.RoleID + " WHERE EventID = " + HttpContext.Session.GetInt32("assigneventid") + " AND UserID = " + HttpContext.Session.GetInt32("assignuserid");
            DBClass.GeneralQuery(sqlQuery);

            DBClass.DBConnection.Close();

            int? eventid = HttpContext.Session.GetInt32("assigneventid");

            return RedirectToPage("/Organizer/AssignRoleSelectUser", new { eventid });
        }
    }
}
