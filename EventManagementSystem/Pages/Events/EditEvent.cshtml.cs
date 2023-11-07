using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Events
{
    public class EditEventModel : PageModel
    {
        [BindProperty]
        public Event EventToUpdate { get; set; }

        public EditEventModel()
        {
            EventToUpdate = new Event();
        }

        int buildingID = 0;

        public IActionResult OnGet(int eventid)
        {
            if (HttpContext.Session.GetString("RoleType") != "Admin" &&
                (HttpContext.Session.GetString("RoleType") == "Presenter" || HttpContext.Session.GetString("RoleType") == "Judge"
                || HttpContext.Session.GetString("RoleType") == "Participant" || HttpContext.Session.GetString("RoleType") == "Organizer"))
            {
                return RedirectToPage("/Attendee/Index");
            }
            else if (HttpContext.Session.GetString("RoleType") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            // Read data from the Event table into each field
            // NOTE: This excludes the "BuildingName" field
            SqlDataReader singleEvent = DBClass.SingleEventReader(eventid);
            while (singleEvent.Read())
            {
                EventToUpdate.EventID = eventid;
                EventToUpdate.EventName = singleEvent["EventName"].ToString();
                EventToUpdate.EventDescription = singleEvent["EventDescription"].ToString();
                EventToUpdate.StartDate = DateTime.Parse(singleEvent["StartDate"].ToString());
                EventToUpdate.EndDate = DateTime.Parse(singleEvent["EndDate"].ToString());
                EventToUpdate.RegistrationDeadline = DateTime.Parse(singleEvent["RegistrationDeadline"].ToString());
                EventToUpdate.Capacity = Int32.Parse(singleEvent["Capacity"].ToString());
                EventToUpdate.Status = singleEvent["Status"].ToString();
                buildingID = Int32.Parse(singleEvent["BuildingID"].ToString());
            }
            DBClass.DBConnection.Close();

            // Retrives the Name from the Building table for the corresponding BuildingID
            string buildingNameQuery = "SELECT [Name] AS BuildingName FROM Building WHERE BuildingID= " + buildingID;
            SqlDataReader buildingNameReader = DBClass.GeneralReaderQuery(buildingNameQuery);
            if (buildingNameReader.Read())
            {
                EventToUpdate.BuildingName = buildingNameReader["BuildingName"].ToString();
            }
            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            // Retrives the BuildingID from the Building table for the corresponding Name
            string buildingIDQuery = "SELECT BuildingID FROM Building WHERE Name= '" + EventToUpdate.BuildingName + "'";
            SqlDataReader buildingIDReader = DBClass.GeneralReaderQuery(buildingIDQuery);
            if (buildingIDReader.Read())
            {
                buildingID = Int32.Parse(buildingIDReader["BuildingID"].ToString());
            }
            DBClass.DBConnection.Close();

            string sqlQuery = "UPDATE Event SET EventName='" + EventToUpdate.EventName
                + "',EventDescription='" + EventToUpdate.EventDescription
                + "',StartDate='" + EventToUpdate.StartDate
                + "',EndDate='" + EventToUpdate.EndDate
                + "',RegistrationDeadline='" + EventToUpdate.RegistrationDeadline
                + "',Capacity='" + EventToUpdate.Capacity
                + "',Status='" + EventToUpdate.Status
                + "',BuildingID=" + buildingID
                + " WHERE EventID=" + EventToUpdate.EventID;
            DBClass.GeneralQuery(sqlQuery);
            DBClass.DBConnection.Close();

            return RedirectToPage("AdminEvent");
        }

    }
}
