using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBook_Library_Service.Models
{
    public class WaitingList
    {


        [Key] // Marks this property as the primary key
        public int WaitingId { get; set; }

        [ForeignKey("User")] // Marks this property as a foreign key to the User class
        public string UserId { get; set; }

        [ForeignKey("Book")] // Marks this property as a foreign key to the Book class
        public int BookId { get; set; }

        public DateTime JoinDate { get; set; } // Date the user joined the waiting list
        public int Position { get; set; } // User's position in the waiting list
        public DateTime? NotificationSentDate { get; set; } // Add this field

        // Navigation properties (optional but useful for querying)
        public virtual Users User { get; set; } // Add this line
        public virtual Book Book { get; set; } // Add this line
    }
}