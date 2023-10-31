namespace EventManagementSystem.Pages.DataClasses
{
    public class User
    {
        public int UserID {  get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public bool IsAttendee { get; set; }

        public bool IsPresenter { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsActive { get; set; }

    }
}
