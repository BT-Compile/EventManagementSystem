namespace EventManagementSystem.Pages.Attendee
{
    public class AttendeeSchedule
    {
        public string? EventName { get; set; }

        public string? EventDescription { get; set; }

        public string? BuildingName { get; set; }

        public int ActivityID { get; set; }

        public string? ActivityName { get; set; }

        public string? ActivityDescription { get; set; }

        public DateTime Date { get; set; }

        public int RoomNumber { get; set; }

        public TimeOnly StartTime { get; set; }
    }
}
