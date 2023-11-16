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

        public int Capacity { get; set; }

        public string? Status { get; set; }

        public string? SpaceName { get; set; }
    }
}
