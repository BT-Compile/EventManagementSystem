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

        public string? AllergyNote { get; set; }

        // Let's keep the IsActive boolean type so that an admin can
        // "activate" or "deactivate" specific users.
        // - Steven
        public bool IsActive { get; set; }


        //Keeping for now so I don't break the fuck out of the solution
        public bool IsAttendee { get; set; }

        public bool IsPresenter { get; set; }

        public bool IsAdmin { get; set; }

    }
}
