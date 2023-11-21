namespace EventManagementSystem.Pages.Users
{
    public class UserData
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Username { get; set; }

        public string? Accomodation { get; set; }

        public string? EventName { get; set; }

        public string? BuildingName { get; set; }

        public string? ActivityName { get; set; }

        public DateTime Date { get; set; }

        public int RoomNumber { get; set; }

        internal static void Add(UserData userData)
        {
            throw new NotImplementedException();
        }
    }
}
