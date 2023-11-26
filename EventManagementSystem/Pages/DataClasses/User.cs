namespace EventManagementSystem.Pages.DataClasses
{
    public class User
    {
        public int UserID {  get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Username { get; set; }

        // we might need to get rid of this because we will be
        // storing the encrypted passwords in the AUTH database.
        // - Steven
        public string? UserPassword { get; set; }

        public string? AllergyID { get; set; }

        public string? Accomodation {  get; set; }

        // Let's keep the IsActive boolean type so that an admin can
        // "activate" or "deactivate" specific users.
        // - Steven
        public bool IsActive { get; set; }

        public int RoleID { get; set; }

        public string? RoleName { get; set; }

        public string? RoleDescription { get; set; }

        public string? RoleType { get; set; }

        public string? Category {  get; set; }
    }
}
