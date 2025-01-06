namespace eBook_Library_Service.Models
{
    public class BorrowRequest
    {
        public int BorrowRequestId { get; set; }
        public int BookId { get; set; }
        public string UserId { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; } // Pending, Approved, Rejected
    }
}