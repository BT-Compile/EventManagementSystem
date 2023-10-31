namespace EventManagementSystem.Pages.DataClasses
{
    public class Room
    {
        public int RoomID { get; set; }

        public string? RoomName { get; set; }

        public string? RoomDescription { get; set; }

        public int MaxCapacity { get; set; }

        public bool IsActive { get; set; }
    }
}
