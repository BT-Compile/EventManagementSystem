using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace EventManagementSystem.Pages.Organizer.Spaces
{
    public class DeleteSpaceModel : PageModel
    {
        [BindProperty]
        public Space SpaceToDelete { get; set; }

        public List<Space> Subspaces { get; set; }

        public DeleteSpaceModel()
        {
            SpaceToDelete = new Space();
            Subspaces = new List<Space>();
        }

        public IActionResult OnGet(int spaceid)
        {
            if (HttpContext.Session.GetString("RoleType") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            HttpContext.Session.SetInt32("tempspaceid", spaceid);

            SqlDataReader singleEvent = DBClass.SingleSpaceReader(spaceid);

            while (singleEvent.Read())
            {
                SpaceToDelete.SpaceID = spaceid;
                SpaceToDelete.Name = singleEvent["Name"].ToString();
            }

            DBClass.DBConnection.Close();

            return Page();
        }

        public IActionResult OnPost()
        {
            //Need to delete subspace relation to event
            string sqlQuery = "SELECT * FROM Space WHERE ParentSpaceID = " + HttpContext.Session.GetInt32("tempspaceid");
            SqlDataReader scheduleReader = DBClass.GeneralReaderQuery(sqlQuery);
            while (scheduleReader.Read())
            {
                Subspaces.Add(new Space
                {
                    SpaceID = Int32.Parse(scheduleReader["SpaceID"].ToString())
                });
            }

            foreach (var space in Subspaces)
            {
                sqlQuery = "DELETE FROM EventSpace WHERE SpaceID = " + space.SpaceID;
                DBClass.GeneralQuery(sqlQuery);
                DBClass.DBConnection.Close();
            }

            //Deletes subspaces from the selected space
            sqlQuery = "DELETE FROM [Space] WHERE ParentSpaceID = " + HttpContext.Session.GetInt32("tempspaceid");
            DBClass.GeneralQuery(sqlQuery);
            DBClass.DBConnection.Close();

            //Deletes the actual space
            sqlQuery = "DELETE FROM [Space] WHERE SpaceID = " + HttpContext.Session.GetInt32("tempspaceid");
            DBClass.GeneralQuery(sqlQuery);
            DBClass.DBConnection.Close();

            //Deletes Space relation to event
            sqlQuery = "DELETE FROM EventSpace WHERE SpaceID = " + HttpContext.Session.GetInt32("tempspaceid");
            DBClass.GeneralQuery(sqlQuery);
            DBClass.DBConnection.Close();

            return RedirectToPage("/Organizer/Spaces/Index");
        }
    }
}

