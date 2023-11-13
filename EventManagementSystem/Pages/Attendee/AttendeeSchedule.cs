namespace EventManagementSystem.Pages.Attendee
{
    public class AttendeeSchedule
    {
        public string? EventName { get; set; }

        public string? EventDescription { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public string? BuildingName { get; set; }

        public int ActivityID { get; set; }

    }
}