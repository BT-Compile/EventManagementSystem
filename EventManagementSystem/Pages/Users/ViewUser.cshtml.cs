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

            // JON USE THE DATACLASS
            string sqlQuery = "";


            SqlDataReader UserViewer = DBClass.GeneralReaderQuery(sqlQuery);

            while (UserViewer.Read())
            {
                Users.UserData.Add(new UserData
                {
                    FirstName = UserViewer["FirstName"].ToString(),
                    LastName = UserViewer["LastName"].ToString(),
                    Email = UserViewer["Email"].ToString(),
                    PhoneNumber = UserViewer["PhoneNumber"].ToString(),
                    Username = UserViewer["Username"].ToString(),
                    Accomodation = UserViewer["Accomodation"].ToString(),
                    EventName = UserViewer["EventName"].ToString(),
                    Date = DateTime.Parse(UserViewer["Date"].ToString()),
                    BuildingName = UserViewer["Name"].ToString(),
                    RoomNumber = int.Parse(UserViewer["RoomNumber"].ToString())
                });
            }
        }
    }
}
