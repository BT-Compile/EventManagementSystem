namespace EventManagementSystem.Pages.DataClasses
{
    public class Activity
    {
        public int ActivityID { get; set; }

        public string? ActivityName { get; set; }

        public string? ActivityDescription { get; set;}

        public DateTime DateAndTime { get; set; }

        public int ExpectedAttendance {  get; set; }

        public bool IsPresentation { get; set; }

        public bool IsMeeting { get; set; }

        public bool IsProgramEvent { get; set; }

        public int? EventID { get; set; }

        public int? PresenterID { get; set; }

        public int? RoomID { get; set; }

        public bool IsActive { get; set; }
    }
}
