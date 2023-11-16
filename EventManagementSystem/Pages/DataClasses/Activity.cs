namespace EventManagementSystem.Pages.DataClasses
{
    public class Activity
    {
        public int ActivityID { get; set; }

        public string? ActivityName { get; set; }

        public string? ActivityDescription { get; set;}

        public DateTime Date { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public string? Type { get; set; }

        public string? Status { get; set; }

        public string? EventName { get; set; }

        public string? BuildingName { get; set; }

        public int? RoomNumber { get; set; }

        public int? RoomID { get; set; }

        public int? EventID {  get; set; }

        public int? ParentActivityID { get; set; }
    }
}
