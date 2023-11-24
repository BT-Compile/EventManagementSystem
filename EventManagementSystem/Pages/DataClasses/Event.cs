namespace EventManagementSystem.Pages.DataClasses
{
    public class Event
    {
        public int EventID { get; set; }

        public string? EventName { get; set; }

        public string? EventDescription { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime RegistrationDeadline { get; set; }

        public int? Capacity { get; set; }

        public int UserID { get; set; }

        public string? Status { get; set; }

        public string? SpaceID { get; set; }

        public string? EventType { get; set;}

        public string? SpaceName { get; set; }

        public string? SpaceAddress { get; set;}

        public string? AmenityName { get; set;}

        public string? AmenityDescription { get; set;}

        public string? AmenityType { get; set;}

        public string? URL { get; set;}

        public string? ParentEventName { get; set; }

        public string? ParentEventID { get; set; }

        public DateTime RequestDate { get; set; }
    }
}
