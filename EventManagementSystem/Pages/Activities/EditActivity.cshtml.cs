using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Activities
{
    public class EditActivityModel : PageModel
    {
        [BindProperty]
        public Activity ActivityToUpdate { get; set; }

        public EditActivityModel() 
        {
            ActivityToUpdate = new Activity();
        }

        public IActionResult OnGet(int activityid)
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

            SqlDataReader singleActivity = DBClass.SingleActivityReader(activityid);

            while (singleActivity.Read())
            {
                ActivityToUpdate.ActivityID = activityid;
                ActivityToUpdate.ActivityName = singleActivity["ActivityName"].ToString();
                ActivityToUpdate.ActivityDescription = singleActivity["ActivityDescription"].ToString();
                ActivityToUpdate.Date = DateTime.Parse(singleActivity["Date"].ToString());
                ActivityToUpdate.StartTime = TimeOnly.Parse(singleActivity["StartTime"].ToString());
                ActivityToUpdate.EndTime = TimeOnly.Parse(singleActivity["EndTime"].ToString());
                ActivityToUpdate.Type = singleActivity["Type"].ToString();
                ActivityToUpdate.Status = singleActivity["Status"].ToString();

                // Check if EventID is DBNull before casting to int
                if (!DBNull.Value.Equals(singleActivity["EventID"]))
                {
                    ActivityToUpdate.EventID = (int)singleActivity["EventID"];
                }
                else
                {
                    ActivityToUpdate.EventID = null;
                }
            }

            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            string sqlQuery;
            if (ActivityToUpdate.EventID != null)
            {
                sqlQuery = "UPDATE Activity SET ActivityName='" + ActivityToUpdate.ActivityName
                    + "',ActivityDescription='" + ActivityToUpdate.ActivityDescription
                    + "',Date='" + ActivityToUpdate.Date
                    + "',StartTime='" + ActivityToUpdate.StartTime
                    + "',EndTime='" + ActivityToUpdate.EndTime
                    + "',Type='" + ActivityToUpdate.Type
                    + "',Status='" + ActivityToUpdate.Status
                    + "',EventID='" + ActivityToUpdate.EventID
                    + "' WHERE ActivityID=" + ActivityToUpdate.ActivityID;
            }
            else
            {
                sqlQuery = "UPDATE Activity SET ActivityName='" + ActivityToUpdate.ActivityName
                    + "',ActivityDescription='" + ActivityToUpdate.ActivityDescription
                    + "',Date='" + ActivityToUpdate.Date
                    + "',StartTime='" + ActivityToUpdate.StartTime
                    + "',EndTime='" + ActivityToUpdate.EndTime
                    + "',Type='" + ActivityToUpdate.Type
                    + "',Status='" + ActivityToUpdate.Status
                    + "',EventID=null"
                    + " WHERE ActivityID=" + ActivityToUpdate.ActivityID;
            }
            

            DBClass.GeneralQuery(sqlQuery);

            DBClass.DBConnection.Close();

            return RedirectToPage("AdminActivity");
        }

    }
}
