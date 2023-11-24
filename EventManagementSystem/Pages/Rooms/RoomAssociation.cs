namespace EventManagementSystem.Pages.Rooms
{
    public class RoomAssociation
    {
        public string? SpaceName { get; set; }
        public int SpaceID { get; set; }
        public int? Capacity { get; set; }
        public string? Address { get; set; }
        public string? EventName { get; set; }
        public string? ParentEventName { get; set; }

        public DateTime EventDate { get; set; }
    }

}