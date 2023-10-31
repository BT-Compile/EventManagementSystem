namespace EventManagementSystem.Pages.DataClasses
{
    public class Event
    {
        public int EventID { get; set; }

        public string? EventName { get; set; }

        public string? EventDescription { get; set; }

        public DateTime EventStartDateAndTime { get; set; }

        public DateTime EventEndDateAndTime { get; set; }

        public string? EventLocation { get; set; }

        public bool IsActive { get; set; }

    }
}
