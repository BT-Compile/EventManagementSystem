using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Events
{
    public class IndexModel : PageModel
    {
        public List<Event> Events { get; set; }

        public IndexModel()
        {
            Events = new List<Event>();
        }

        public void OnGet()
        {
            string sqlQuery = "SELECT * FROM Event";

            SqlDataReader eventReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (eventReader.Read())
            {
                Events.Add(new Event
                {
                    EventName = eventReader["EventName"].ToString(),
                    EventDescription = eventReader["EventDescription"].ToString(),
                    StartDate = (DateTime)eventReader["StartDate"],
                    EndDate = (DateTime)eventReader["EndDate"],
                    RegistrationDeadline = (DateTime)eventReader["RegistrationDeadline"],
                    Capacity = Int32.Parse(eventReader["Capacity"].ToString()),
                    Status = eventReader["Status"].ToString()
                });
            }

            DBClass.DBConnection.Close();
        }
    }
}
