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

            string sqlQuery = "SELECT * FROM Activity WHERE EventID = " + eventID + " ORDER BY DateAndTime";
            SqlDataReader scheduleViewer = DBClass.GeneralReaderQuery(sqlQuery);

            while (scheduleViewer.Read())
            {
                Activities.Add(new Activity
                {
                    ActivityName = scheduleViewer["ActivityName"].ToString(),
                    ActivityDescription = scheduleViewer["ActivityDescription"].ToString(),
                    DateAndTime = (DateTime)scheduleViewer["DateAndTime"],
                    IsPresentation = (bool)scheduleViewer["IsPresentation"],
                    IsMeeting = (bool)scheduleViewer["IsMeeting"],
                    IsProgramEvent = (bool)scheduleViewer["IsProgramEvent"]
                });
            }
        }


    }
}
