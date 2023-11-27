using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace EventManagementSystem.Pages.Organizer
{
    public class RoleAssociationsModel : PageModel
    {
        public List<EventUser> Users { get; set; }

        public List<EventUser> SubUsers { get; set; }

        [BindProperty]
        public Event EventNames { get; set; }

        [BindProperty]
        public string? InputString { get; set; }

        public string[]? Keywords { get; set; }

        [BindProperty]
        public bool test { get; set; }

        public RoleAssociationsModel()
        {
            Users = new List<EventUser>();
            SubUsers = new List<EventUser>();
            EventNames = new Event();
            test = false;
        }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("RoleType") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            string sqlQuery = "SELECT Event.EventID, Event.EventName, Event.EventDescription, Event.StartDate, Event.EndDate, Event.RegistrationDeadline, EventRegister.RegistrationDate, Event.Capacity, Event.Type, " +
                "Event.Status, Event.OrganizerID, Event.ParentEventID, [User].UserID, concat_ws(' ', [User].FirstName, [User].LastName) AS FullName, Role.RoleID, Role.Name, Role.Description " +
                "FROM  UserRole INNER JOIN Role ON UserRole.RoleID = Role.RoleID INNER JOIN Event LEFT OUTER JOIN EventRegister ON Event.EventID = EventRegister.EventID INNER JOIN " +
                "[User] ON EventRegister.UserID = [User].UserID ON UserRole.UserID = [User].UserID " +
                "WHERE Event.OrganizerID = " + HttpContext.Session.GetString("userid") + " AND Event.ParentEventID IS NOT NULL " +
                "AND [User].IsActive = 'true' ORDER BY EventRegister.RegistrationDate, [Event].EventName, Role.Name DESC";

            SqlDataReader userReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (userReader.Read())
            {
                Users.Add(new EventUser
                {
                    UserID = Int32.Parse(userReader["UserID"].ToString()),
                    FirstName = userReader["FullName"].ToString(),
                    RoleType = userReader["Name"].ToString(),
                    EventID = Int32.Parse(userReader["EventID"].ToString()),
                    EventName = userReader["EventName"].ToString(),
                    RegistrationDate = (DateTime)userReader["RegistrationDate"],
                    RoleID = Int32.Parse(userReader["RoleID"].ToString())
                });
            }

            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost(string keyword)
        {
            test = true;
            Keywords = Regex.Split(InputString, @"\s+");
            string sqlQuery;
            int? eventid = HttpContext.Session.GetInt32("eventid");

            for (int i = 0; i < Keywords.Length; i++)
            {
                keyword = Keywords[i];

                sqlQuery = "SELECT Event.EventID, Event.EventName, Event.EventDescription, Event.StartDate, Event.EndDate, Event.RegistrationDeadline, EventRegister.RegistrationDate, Event.Capacity, Event.Type, " +
                "Event.Status, Event.OrganizerID, Event.ParentEventID, [User].UserID, concat_ws(' ', [User].FirstName, [User].LastName) AS FullName, Role.RoleID, Role.Name, Role.Description " +
                "FROM  UserRole INNER JOIN Role ON UserRole.RoleID = Role.RoleID INNER JOIN Event LEFT OUTER JOIN EventRegister ON Event.EventID = EventRegister.EventID INNER JOIN " +
                "[User] ON EventRegister.UserID = [User].UserID ON UserRole.UserID = [User].UserID " +
                "WHERE Event.OrganizerID = " + HttpContext.Session.GetString("userid") + " AND Event.ParentEventID IS NOT NULL " +
                               " AND [User].IsActive = 'true' " +
                                "AND ([User].FirstName LIKE '%" + keyword + "%' OR [User].LastName LIKE '%" + keyword + "%' " +
                                "OR [User].Username LIKE '%" + keyword + "%') ORDER BY EventRegister.RegistrationDate, [Event].EventName, Role.Name DESC";

                SqlDataReader subuserReader = DBClass.GeneralReaderQuery(sqlQuery);

                while (subuserReader.Read())
                {
                    Users.Add(new EventUser
                    {
                        UserID = Int32.Parse(subuserReader["UserID"].ToString()),
                        FirstName = subuserReader["FullName"].ToString(),
                        RoleType = subuserReader["Name"].ToString(),
                        EventID = Int32.Parse(subuserReader["EventID"].ToString()),
                        EventName = subuserReader["EventName"].ToString(),
                        RegistrationDate = (DateTime)subuserReader["RegistrationDate"]
                    });
                }
            }

            DBClass.DBConnection.Close();
            return Page();
        }
    }
}
