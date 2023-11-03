using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.EventSchedule
{
    public class ScheduleModel : PageModel
    {
        [BindProperty]
        public string? EventName { get; set; }

        public List<Activity> Activities { get; set; }

        public ScheduleModel()
        {
            Activities = new List<Activity>();
        }

        public void OnGet(int eventID)
        {
            EventName = DBClass.GetEventName(eventID);

            string sqlQuery = "SELECT * FROM Activity WHERE EventID = " + eventID + " ORDER BY Date";
            SqlDataReader scheduleViewer = DBClass.GeneralReaderQuery(sqlQuery);

            while (scheduleViewer.Read())
            {
                Activities.Add(new Activity
                {
                    ActivityName = scheduleViewer["ActivityName"].ToString(),
                    ActivityDescription = scheduleViewer["ActivityDescription"].ToString(),
                    Date = DateTime.Parse(scheduleViewer["Date"].ToString()),
                    StartTime = TimeOnly.Parse(scheduleViewer["StartTime"].ToString())
                });
            }
        }


    }
}
