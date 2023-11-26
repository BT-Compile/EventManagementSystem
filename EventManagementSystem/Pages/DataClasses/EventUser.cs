namespace EventManagementSystem.Pages.DataClasses
{
    public class EventUser
    {
        public int EventID { get; set; }

        public string? EventName { get; set; }

        public string? EventDescription { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime RegistrationDeadline { get; set; }

        public int? Capacity { get; set; }

        public string? Status { get; set; }

        public string? SpaceID { get; set; }

        public string? EventType { get; set; }

        public string? SpaceName { get; set; }

        public string? SpaceAddress { get; set; }

        public string? AmenityName { get; set; }

        public string? AmenityDescription { get; set; }

        public string? AmenityType { get; set; }

        public string? URL { get; set; }

        public string? ParentEventName { get; set; }

        public string? ParentSpaceName { get; set; }

        public string? ParentEventID { get; set; }

        public int TempParentID { get; set; }

        public DateTime RequestDate { get; set; }

        public DateTime RegistrationDate { get; set; }

        public int UserID { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Username { get; set; }

        public string? Accomodation { get; set; }

        public int RoleID { get; set; }

        public string? RoleType { get; set; }

        public string? Category { get; set; }

        public bool IsActive { get; set; }

        public string? RoleName { get; set; }

        public string? RoleDescription { get; set; }
    }
}
