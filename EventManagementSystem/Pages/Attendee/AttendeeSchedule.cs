namespace EventManagementSystem.Pages.Attendee
{
    public class AttendeeSchedule
    {
        public string? EventName { get; set; }

        public string? EventDescription { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public string? ActivityName { get; set; }

        public string? ActivityDescription { get; set; }

        public DateOnly ActivityDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public string? BuildingName { get; set; }

        public int RoomNumber { get; set; }

        public int ActivityID { get; set; }

    }
}
