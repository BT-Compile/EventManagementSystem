namespace EventManagementSystem.Pages.Attendee
{
    public class AttendeeSchedule
    {
        public string? EventName { get; set; }

        public string? EventDescription { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string? SpaceID { get; set; }

        public int EventID { get; set; }

    }
}