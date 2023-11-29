using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace EventManagementSystem.Pages.Organizer
{
    public class ViewCheckInModel : PageModel
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

        public ViewCheckInModel()
        {
            Users = new List<EventUser>();
            SubUsers = new List<EventUser>();
            EventNames = new Event();
            test = false;
        }

        public IActionResult OnGet(int eventid)
        {
            if (HttpContext.Session.GetString("RoleType") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            HttpContext.Session.SetInt32("eventid", eventid);

            string sqlQuery = "SELECT Event.*, [User].IsActive, [User].UserID, concat_ws(' ', [User].FirstName, [User].LastName) as FullName, EventCheckIn.CheckInDateTime, Role.Name " +
                              "FROM Event INNER JOIN EventCheckIn ON Event.EventID = EventCheckIn.EventID INNER JOIN [User] ON EventCheckIn.UserID = [User].UserID INNER JOIN " +
                              "UserRole ON [User].UserID = UserRole.UserID INNER JOIN Role ON UserRole.RoleID = Role.RoleID " +
                              "WHERE Event.OrganizerID = " + HttpContext.Session.GetString("userid") +
                               " AND ([Event].EventID = " + eventid + ") AND [User].IsActive = 'true' ORDER BY EventCheckIn.CheckInDateTime, Role.Name DESC";

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
                    RegistrationDate = (DateTime)userReader["CheckInDateTime"]
                });
            }

            DBClass.DBConnection.Close();

            sqlQuery = "SELECT * FROM Event WHERE EventID = " + eventid;
            SqlDataReader singleEvent = DBClass.GeneralReaderQuery(sqlQuery);

            while (singleEvent.Read())
            {
                EventNames.EventID = eventid;
                EventNames.EventName = singleEvent["EventName"].ToString();
            }

            HttpContext.Session.SetString("eventname", EventNames.EventName);
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

                sqlQuery = "SELECT Event.*, [User].IsActive, [User].UserID, concat_ws(' ', [User].FirstName, [User].LastName) as FullName, EventCheckIn.CheckInDateTime, Role.Name " +
                              "FROM Event INNER JOIN EventCheckIn ON Event.EventID = EventCheckIn.EventID INNER JOIN [User] ON EventCheckIn.UserID = [User].UserID INNER JOIN " +
                              "UserRole ON [User].UserID = UserRole.UserID INNER JOIN Role ON UserRole.RoleID = Role.RoleID " +
                              "WHERE Event.OrganizerID = " + HttpContext.Session.GetString("userid") +
                               " AND ([Event].EventID = " + eventid + ") AND [User].IsActive = 'true' AND " +
                                "([User].FirstName LIKE '%" + keyword + "%' OR [User].LastName LIKE '%" + keyword + "%') ORDER BY EventCheckIn.CheckInDateTime, Role.Name DESC";

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
                        RegistrationDate = (DateTime)userReader["CheckInDateTime"]
                    });
                }
                DBClass.DBConnection.Close();
            }
            DBClass.DBConnection.Close();
            return Page();
        }
    }
}