using EventManagementSystem.Pages.DataClasses;
using EventManagementSystem.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace EventManagementSystem.Pages.Rooms
{
    public class AdminRoomModel : PageModel
    {
        [BindProperty]
        public bool HasPosted { get; set; }

        [BindProperty]
        public string? InputString { get; set; }

        public string[]? Keywords { get; set; }

        public List<Space> SubSpaces { get; set; }

        public List<Space> Spaces { get; set; }

        public AdminRoomModel()
        {
            SubSpaces = new List<Space>();
            Spaces = new List<Space>();
            HasPosted = false;
        }
        public IActionResult OnGet()
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

            string sqlQuery = "SELECT Space.SpaceID, Space.Name, Space.Address, Space.Capacity, Space_1.Name AS ParentSpaceName FROM Space INNER JOIN " +
                   "Space AS Space_1 ON Space.ParentSpaceID = Space_1.SpaceID WHERE(Space.ParentSpaceID IS NOT NULL) " +
                   "ORDER BY Space_1.Name";
            SqlDataReader subspaceReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (subspaceReader.Read())
            {
                SubSpaces.Add(new Space
                {
                    SpaceID = Int32.Parse(subspaceReader["SpaceID"].ToString()),
                    Name = subspaceReader["Name"].ToString(),
                    Address = subspaceReader["Address"].ToString(),
                    Capacity = Int32.Parse(subspaceReader["Capacity"].ToString()),
                    ParentSpaceName = subspaceReader["ParentSpaceName"].ToString()
                });
            }

            sqlQuery = "SELECT SpaceID, [Name], [Address], Capacity FROM  [Space] WHERE ParentSpaceID IS NULL ORDER BY [Name]";
            SqlDataReader spaceReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (spaceReader.Read())
            {
                Spaces.Add(new Space
                {
                    SpaceID = Int32.Parse(spaceReader["SpaceID"].ToString()),
                    Name = spaceReader["Name"].ToString(),
                    Address = spaceReader["Address"].ToString(),
                    Capacity = Int32.Parse(spaceReader["Capacity"].ToString())
                });
            }

            DBClass.DBConnection.Close();

            return Page();
        }

        //Post Request for the search bar, returns Activities from search keywords
        public IActionResult OnPost()
        {
            HasPosted = true;
            Keywords = Regex.Split(InputString, @"\s+");
            string keyword, sqlQuery;

            for (int i = 0; i < Keywords.Length; i++)
            {
                keyword = Keywords[i];

                // query to do a CASE INSENSITIVE search for a keyword in the Space table for Main Spaces
                sqlQuery = "SELECT SpaceID, [Name], [Address], Capacity FROM  [Space] WHERE ParentSpaceID IS NULL " +
                    "AND ([Name] LIKE '%" + keyword + "%' OR [Address] LIKE '%" + keyword + "%') ORDER BY [Name]";

                SqlDataReader spaceReader = DBClass.GeneralReaderQuery(sqlQuery);

                while (spaceReader.Read())
                {
                    Spaces.Add(new Space
                    {
                        SpaceID = Int32.Parse(spaceReader["SpaceID"].ToString()),
                        Name = spaceReader["Name"].ToString(),
                        Address = spaceReader["Address"].ToString(),
                        Capacity = Int32.Parse(spaceReader["Capacity"].ToString())
                    });
                }
                DBClass.DBConnection.Close();

                // query to do a CASE INSENSITIVE search for a keyword in the Space table for SUB SPACES
                sqlQuery = "SELECT Space.SpaceID, Space.Name, Space.Address, Space.Capacity, Space_1.Name AS ParentSpaceName FROM Space INNER JOIN " +
                              "Space AS Space_1 ON Space.ParentSpaceID = Space_1.SpaceID WHERE (Space.ParentSpaceID IS NOT NULL) " +
                              "AND (Space.Name LIKE '%" + keyword + "%' OR Space.[Address] LIKE '%" + keyword + "%' OR Space_1.Name LIKE '%" + keyword + "%' OR Space_1.[Address] LIKE '%" + keyword + "%') " +
                              "ORDER BY Space_1.Name ";
                              
                SqlDataReader subspaceReader = DBClass.GeneralReaderQuery(sqlQuery);

                while (subspaceReader.Read())
                {
                    SubSpaces.Add(new Space
                    {
                        SpaceID = Int32.Parse(subspaceReader["SpaceID"].ToString()),
                        Name = subspaceReader["Name"].ToString(),
                        Address = subspaceReader["Address"].ToString(),
                        Capacity = Int32.Parse(subspaceReader["Capacity"].ToString()),
                        ParentSpaceName = subspaceReader["ParentSpaceName"].ToString()
                    });
                }

                DBClass.DBConnection.Close();
            }

            return Page();
        }
    }
}
