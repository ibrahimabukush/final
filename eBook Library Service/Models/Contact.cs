namespace eBook_Library_Service.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime SubmissionDate { get; set; } = DateTime.UtcNow;
    }
}