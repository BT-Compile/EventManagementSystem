namespace EventManagementSystem.Pages.DataClasses
{
    public class Transaction
    {
        public int TransactionID { get; set; }

        public decimal Total {  get; set; }

        public DateTime TransactionDate { get; set; }

        public string? TransactionNotes { get; set; }

        public string? Status { get; set;}

        public int UserID { get; set; }

        public int DiscountID { get; set; }
    }
}
