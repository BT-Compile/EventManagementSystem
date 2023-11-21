using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace EventManagementSystem.Pages.Users
{
    public class ViewUserModel : PageModel
    {
        [BindProperty]
        public string? UserName { get; set; }

        public List<UserData> UserData { get; set; }

        public ViewUserModel()
        {
            UserData = new List<UserData>();
        }

        public void OnGet(int userID)
        {
            UserName = DBClass.GetUserName(userID);

            string sqlQuery = "SELECT U.FirstName, U.LastName, U.Email, U.PhoneNumber, U.Username, U.Accomodation, E.EventName, E.StartDate, E.EndDate " +
                "FROM [User] U LEFT JOIN EventRegister ER ON U.UserID = ER.UserID LEFT JOIN [Event] E ON ER.EventID = E.EventID WHERE U.UserID = " + userID;

            SqlDataReader UserViewer = DBClass.GeneralReaderQuery(sqlQuery);

            while (UserViewer.Read())
            {
                UserData.Add(new UserData
                {
                    FirstName = UserViewer["FirstName"].ToString(),
                    LastName = UserViewer["LastName"].ToString(),
                    Email = UserViewer["Email"].ToString(),
                    PhoneNumber = UserViewer["PhoneNumber"].ToString(),
                    Username = UserViewer["Username"].ToString(),
                    Accomodation = UserViewer["Accomodation"].ToString(),
                    EventName = UserViewer["EventName"].ToString(),
                });
            }
        }
    }
}
