using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace EventManagementSystem.Pages.Organizer
{
    public class ParticipantListModel : PageModel
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

        public ParticipantListModel()
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

            string sqlQuery = "SELECT Event.*, [User].UserID, concat_ws(' ', [User].FirstName, [User].LastName) as FullName, [User].Email, [User].PhoneNumber, [User].Username, " +
                              "[User].Accomodation, [User].IsActive, Role.*, Allergy.Category, EventRegister.RegistrationDate FROM  Event INNER JOIN EventRegister ON Event.EventID " +
                              "= EventRegister.EventID INNER JOIN [User] ON EventRegister.UserID = [User].UserID INNER JOIN UserRole ON [User].UserID = UserRole.UserID INNER JOIN Role " +
                              "ON UserRole.RoleID = Role.RoleID LEFT OUTER JOIN Allergy ON [User].AllergyID = Allergy.AllergyID LEFT OUTER JOIN AllergyBridge ON [User].UserID = AllergyBridge.UserID " +
                              "AND Allergy.AllergyID = AllergyBridge.AllergyID WHERE Event.OrganizerID = " + HttpContext.Session.GetString("userid") +
                               " AND ([Event].EventID = " + eventid + ") AND [User].IsActive = 'true' AND Role.Name = 'Participant' ORDER BY EventRegister.RegistrationDate, [Event].EventName, Role.Name DESC";

            SqlDataReader userReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (userReader.Read())
            {
                Users.Add(new EventUser
                {
                    UserID = Int32.Parse(userReader["UserID"].ToString()),
                    FirstName = userReader["FullName"].ToString(),
                    Email = userReader["Email"].ToString(),
                    PhoneNumber = userReader["PhoneNumber"].ToString(),
                    Username = userReader["Username"].ToString(),
                    Category = userReader["Category"].ToString(),
                    Accomodation = userReader["Accomodation"].ToString(),
                    RoleType = userReader["Name"].ToString(),
                    EventID = Int32.Parse(userReader["EventID"].ToString()),
                    EventName = userReader["EventName"].ToString(),
                    RegistrationDate = (DateTime)userReader["RegistrationDate"]
                });
            }

            DBClass.DBConnection.Close();

            sqlQuery = "SELECT Event.*, [User].UserID, concat_ws(' ', [User].FirstName, [User].LastName) as FullName, [User].Email, [User].PhoneNumber, [User].Username, " +
                              "[User].Accomodation, [User].IsActive, Role.*, Allergy.Category, EventRegister.RegistrationDate FROM  Event INNER JOIN EventRegister ON Event.EventID " +
                              "= EventRegister.EventID INNER JOIN [User] ON EventRegister.UserID = [User].UserID INNER JOIN UserRole ON [User].UserID = UserRole.UserID INNER JOIN Role " +
                              "ON UserRole.RoleID = Role.RoleID LEFT OUTER JOIN Allergy ON [User].AllergyID = Allergy.AllergyID LEFT OUTER JOIN AllergyBridge ON [User].UserID = AllergyBridge.UserID " +
                              "AND Allergy.AllergyID = AllergyBridge.AllergyID WHERE Event.OrganizerID = " + HttpContext.Session.GetString("userid") +
                               " AND ([Event].ParentEventID = " + eventid + ") AND [User].IsActive = 'true' ORDER BY EventRegister.RegistrationDate, [Event].EventName, Role.Name DESC";

            SqlDataReader subuserReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (subuserReader.Read())
            {
                SubUsers.Add(new EventUser
                {
                    UserID = Int32.Parse(subuserReader["UserID"].ToString()),
                    FirstName = subuserReader["FullName"].ToString(),
                    Email = subuserReader["Email"].ToString(),
                    PhoneNumber = subuserReader["PhoneNumber"].ToString(),
                    Username = subuserReader["Username"].ToString(),
                    Category = subuserReader["Category"].ToString(),
                    Accomodation = subuserReader["Accomodation"].ToString(),
                    RoleType = subuserReader["Name"].ToString(),
                    EventID = Int32.Parse(subuserReader["EventID"].ToString()),
                    EventName = subuserReader["EventName"].ToString(),
                    RegistrationDate = (DateTime)subuserReader["RegistrationDate"]
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

                sqlQuery = "SELECT Event.*, [User].UserID, concat_ws(' ', [User].FirstName, [User].LastName) as FullName, [User].Email, [User].PhoneNumber, [User].Username, " +
                              "[User].Accomodation, [User].IsActive, Role.*, Allergy.Category, EventRegister.RegistrationDate FROM  Event INNER JOIN EventRegister ON Event.EventID " +
                              "= EventRegister.EventID INNER JOIN [User] ON EventRegister.UserID = [User].UserID INNER JOIN UserRole ON [User].UserID = UserRole.UserID INNER JOIN Role " +
                              "ON UserRole.RoleID = Role.RoleID LEFT OUTER JOIN Allergy ON [User].AllergyID = Allergy.AllergyID LEFT OUTER JOIN AllergyBridge ON [User].UserID = AllergyBridge.UserID " +
                              "AND Allergy.AllergyID = AllergyBridge.AllergyID WHERE Event.OrganizerID = " + HttpContext.Session.GetString("userid") +
                               " AND ([Event].EventID = " + eventid + ") AND [User].IsActive = 'true' " +
                                "AND ([User].FirstName LIKE '%" + keyword + "%' OR [User].LastName LIKE '%" + keyword + "%' " +
                                "OR [User].Username LIKE '%" + keyword + "%') ORDER BY EventRegister.RegistrationDate, [Event].EventName, Role.Name DESC";

                SqlDataReader userReader = DBClass.GeneralReaderQuery(sqlQuery);

                while (userReader.Read())
                {
                    Users.Add(new EventUser
                    {
                        UserID = Int32.Parse(userReader["UserID"].ToString()),
                        FirstName = userReader["FullName"].ToString(),
                        Email = userReader["Email"].ToString(),
                        PhoneNumber = userReader["PhoneNumber"].ToString(),
                        Username = userReader["Username"].ToString(),
                        Category = userReader["Category"].ToString(),
                        Accomodation = userReader["Accomodation"].ToString(),
                        RoleType = userReader["Name"].ToString(),
                        EventID = Int32.Parse(userReader["EventID"].ToString()),
                        EventName = userReader["EventName"].ToString(),
                        RegistrationDate = (DateTime)userReader["RegistrationDate"]
                    });
                }
                DBClass.DBConnection.Close();

                sqlQuery = "SELECT Event.*, [User].UserID, concat_ws(' ', [User].FirstName, [User].LastName) as FullName, [User].Email, [User].PhoneNumber, [User].Username, " +
                              "[User].Accomodation, [User].IsActive, Role.*, Allergy.Category, EventRegister.RegistrationDate FROM  Event INNER JOIN EventRegister ON Event.EventID " +
                              "= EventRegister.EventID INNER JOIN [User] ON EventRegister.UserID = [User].UserID INNER JOIN UserRole ON [User].UserID = UserRole.UserID INNER JOIN Role " +
                              "ON UserRole.RoleID = Role.RoleID LEFT OUTER JOIN Allergy ON [User].AllergyID = Allergy.AllergyID LEFT OUTER JOIN AllergyBridge ON [User].UserID = AllergyBridge.UserID " +
                              "AND Allergy.AllergyID = AllergyBridge.AllergyID WHERE Event.OrganizerID = " + HttpContext.Session.GetString("userid") +
                               " AND ([Event].ParentEventID = " + eventid + ") AND [User].IsActive = 'true' " +
                                "AND ([User].FirstName LIKE '%" + keyword + "%' OR [User].LastName LIKE '%" + keyword + "%' " +
                                "OR [User].Username LIKE '%" + keyword + "%') ORDER BY EventRegister.RegistrationDate, [Event].EventName, Role.Name DESC";

                SqlDataReader subuserReader = DBClass.GeneralReaderQuery(sqlQuery);

                while (subuserReader.Read())
                {
                    SubUsers.Add(new EventUser
                    {
                        UserID = Int32.Parse(subuserReader["UserID"].ToString()),
                        FirstName = subuserReader["FullName"].ToString(),
                        Email = subuserReader["Email"].ToString(),
                        PhoneNumber = subuserReader["PhoneNumber"].ToString(),
                        Username = subuserReader["Username"].ToString(),
                        Category = subuserReader["Category"].ToString(),
                        Accomodation = subuserReader["Accomodation"].ToString(),
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
