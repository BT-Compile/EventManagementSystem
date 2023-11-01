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
            if (HttpContext.Session.GetString("usertype") != "Admin" && HttpContext.Session.GetString("usertype") == "Attendee")
            {
                return RedirectToPage("/Attendee/Index");
            }
            else if (HttpContext.Session.GetString("usertype") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            SqlDataReader singleActivity = DBClass.SingleActivityReader(activityid);

            while (singleActivity.Read())
            {
                ActivityToUpdate.ActivityID = activityid;
                ActivityToUpdate.ActivityName = singleActivity["ActivityName"].ToString();
                ActivityToUpdate.ActivityDescription = singleActivity["ActivityDescription"].ToString();
                ActivityToUpdate.DateAndTime = (DateTime)singleActivity["DateAndTime"];
                ActivityToUpdate.IsPresentation = (bool)singleActivity["IsPresentation"];
                ActivityToUpdate.IsMeeting = (bool)(bool)singleActivity["IsMeeting"];
                ActivityToUpdate.IsProgramEvent = (bool)singleActivity["IsProgramEvent"];

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
                    + "',DateAndTime='" + ActivityToUpdate.DateAndTime
                    + "',IsPresentation='" + ActivityToUpdate.IsPresentation
                    + "',IsMeeting='" + ActivityToUpdate.IsMeeting
                    + "',IsProgramEvent='" + ActivityToUpdate.IsProgramEvent
                    + "',EventID='" + ActivityToUpdate.EventID
                    + "' WHERE ActivityID=" + ActivityToUpdate.ActivityID;
            }
            else
            {
                sqlQuery = "UPDATE Activity SET ActivityName='" + ActivityToUpdate.ActivityName
                    + "',ActivityDescription='" + ActivityToUpdate.ActivityDescription
                    + "',DateAndTime='" + ActivityToUpdate.DateAndTime
                    + "',IsPresentation='" + ActivityToUpdate.IsPresentation
                    + "',IsMeeting='" + ActivityToUpdate.IsMeeting
                    + "',IsProgramEvent='" + ActivityToUpdate.IsProgramEvent
                    + "',EventID=null"
                    + " WHERE ActivityID=" + ActivityToUpdate.ActivityID;
            }
            

            DBClass.GeneralQuery(sqlQuery);

            DBClass.DBConnection.Close();

            return RedirectToPage("AdminActivity");
        }

    }
}
