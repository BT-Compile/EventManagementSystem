namespace EventManagementSystem.Pages.DataClasses
{
    public class Room
    {
        public int RoomID { get; set; }

        public int RoomNumber { get; set; }

        public int Capacity { get; set; }

        public int BuildingID { get; set; }

        public string? BuildingName { get; set; }
    }
}
