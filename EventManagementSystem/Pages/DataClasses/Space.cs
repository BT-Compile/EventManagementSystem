namespace EventManagementSystem.Pages.DataClasses
{
    public class Space
    {
        public int SpaceID { get; set; }

        public string? Name { get; set; }

        public string? Address { get; set; }

        public int? Capacity { get; set; }

        public int? ParentSpaceID { get; set; }

        public int? LocationID { get; set; }

        public string? ParentSpaceName { get; set; }
    }
}
