using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using EventManagementSystem.Pages.Rooms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace EventManagementSystem.Pages.Organizer.Spaces
{
    public class RoomAssociationsModel : PageModel
    {
        [BindProperty]
        public bool HasPosted { get; set; }

        [BindProperty]
        public string? InputString { get; set; }

        public string[]? Keywords { get; set; }

        public List<RoomAssociation> Spaces { get; set; }

        public List<RoomAssociation> Subspaces { get; set; }

        public RoomAssociationsModel()
        {
            Spaces = new List<RoomAssociation>();
            Subspaces = new List<RoomAssociation>();
        }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("RoleType") != "Organizer" &&
                (HttpContext.Session.GetString("RoleType") == "Presenter" || HttpContext.Session.GetString("RoleType") == "Judge"
                || HttpContext.Session.GetString("RoleType") == "Participant" || HttpContext.Session.GetString("RoleType") == "Admin"))
            {
                return RedirectToPage("/Attendee/Index");
            }
            else if (HttpContext.Session.GetString("RoleType") == null)
            {
                return RedirectToPage("/Login/Index");
            }

            string sqlQuery = "SELECT [Space].*, Event.* FROM  Event INNER JOIN EventSpace ON Event.EventID = EventSpace.EventID INNER JOIN [Space] " +
            "ON EventSpace.SpaceID = [Space].SpaceID WHERE (Event.OrganizerID = " + HttpContext.Session.GetString("userid") + " OR Space.CreatorID = " + HttpContext.Session.GetString("userid") + ")";

            SqlDataReader spaceReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (spaceReader.Read())
            {
                Spaces.Add(new RoomAssociation
                {
                    SpaceID = Int32.Parse(spaceReader["SpaceID"].ToString()),
                    SpaceName = spaceReader["Name"].ToString(),
                    Address = spaceReader["Address"].ToString(),
                    Capacity = Int32.Parse(spaceReader["Capacity"].ToString()),
                    EventDate = (DateTime)spaceReader["StartDate"],
                    EventName = spaceReader["EventName"].ToString()
                });
            }
            DBClass.DBConnection.Close();
            return Page();
        }

        public IActionResult OnPost()
        {
            HasPosted = true;
            Keywords = Regex.Split(InputString, @"\s+");
            string keyword, sqlQuery;

            for (int i = 0; i < Keywords.Length; i++)
            {
                keyword = Keywords[i];

                // query to do a CASE INSENSITIVE search for a keyword in the Space table for Main Spaces
                sqlQuery = "SELECT [Space].*, Event.* FROM  Event INNER JOIN EventSpace ON Event.EventID = EventSpace.EventID INNER JOIN [Space] " +
                            "ON EventSpace.SpaceID = [Space].SpaceID WHERE (Event.OrganizerID = " + HttpContext.Session.GetString("userid") + " OR Space.CreatorID = " + HttpContext.Session.GetString("userid") + ") " + 
                            " AND ([Name] LIKE '%" + keyword + "%' OR [Address] LIKE '%" + keyword + "%' OR " +
                              "EventName LIKE '%" + keyword + "%')  ORDER BY StartDate, [Name]";

                SqlDataReader spaceReader = DBClass.GeneralReaderQuery(sqlQuery);

                while (spaceReader.Read())
                {
                    Spaces.Add(new RoomAssociation
                    {
                        SpaceID = Int32.Parse(spaceReader["SpaceID"].ToString()),
                        SpaceName = spaceReader["Name"].ToString(),
                        Address = spaceReader["Address"].ToString(),
                        Capacity = Int32.Parse(spaceReader["Capacity"].ToString()),
                        EventDate = (DateTime)spaceReader["StartDate"],
                        EventName = spaceReader["EventName"].ToString()
                    });
                }
                DBClass.DBConnection.Close();
            }

            return Page();
        }
    }
}
