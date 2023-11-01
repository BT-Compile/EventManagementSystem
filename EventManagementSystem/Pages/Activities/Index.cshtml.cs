using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Activities
{
    public class IndexModel : PageModel
    {
        public List<Activity> Activities { get; set; }

        public IndexModel()
        {
            Activities = new List<Activity>();
        }

        public void OnGet()
        {
            string sqlQuery = "SELECT * FROM Activity";
            SqlDataReader activityReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (activityReader.Read())
            {
                // Check if EventID is NULL
                int? eventID = activityReader["EventID"] is DBNull ? (int?)null : (int)activityReader["EventID"];

                Activities.Add(new Activity
                {
                    ActivityName = activityReader["ActivityName"].ToString(),
                    ActivityDescription = activityReader["ActivityDescription"].ToString(),
                    DateAndTime = (DateTime)activityReader["DateAndTime"]
                });
            }

            DBClass.DBConnection.Close();
        }
    }
}
