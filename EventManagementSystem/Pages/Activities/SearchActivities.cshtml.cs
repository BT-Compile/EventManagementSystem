using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;

namespace EventManagementSystem.Pages.Activities
{
    public class SearchActivitiesModel : PageModel
    {
        [BindProperty]
        public bool HasPosted { get; set; }

        [BindProperty]
        public string? InputString { get; set; }

        public string[]? Keywords { get; set; }

        [BindProperty]
        public List<Activity> Activities { get; set; }

        public SearchActivitiesModel()
        {
            Activities = new List<Activity>();
            HasPosted = false;
        }

        public void OnGet()
        {
            if (InputString != null)
            {
                OnPost();
            }
            
        }

        public IActionResult OnPost()
        {
            HasPosted = true;
            Keywords = Regex.Split(InputString, @"\s+");
            string keyword, sqlQuery;

            for (int i = 0; i < Keywords.Length; i++)
            {
                keyword = Keywords[i];
                
                // query to do a CASE INSENSITIVE search for a keyword in the Activity table 
                sqlQuery = "SELECT * FROM Activity WHERE ActivityDescription " +
                    "COLLATE Latin1_General_CI_AI LIKE '%" + keyword + "%'";

                SqlDataReader activityReader = DBClass.GeneralReaderQuery(sqlQuery);

                while (activityReader.Read())
                {
                    int activityID = Int32.Parse(activityReader["ActivityID"].ToString());

                    // Check if an activity with the same ID already exists in the list
                    if (!Activities.Any(a => a.ActivityID == activityID))
                    {
                        Activities.Add(new Activity
                        {
                            ActivityID = activityID,
                            ActivityName = activityReader["ActivityName"].ToString(),
                            ActivityDescription = activityReader["ActivityDescription"].ToString(),
                            Date = DateTime.Parse(activityReader["Date"].ToString()),
                            StartTime = TimeOnly.Parse(activityReader["StartTime"].ToString()),
                            EndTime = TimeOnly.Parse(activityReader["EndTime"].ToString()),
                            Type = activityReader["ActivityName"].ToString(),
                            Status = activityReader["ActivityName"].ToString()
                        });
                    }
                }
            }

            return Page();
        }
    }
}
