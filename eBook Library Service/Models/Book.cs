using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBook_Library_Service.Models
{
    public class Book
    {
        public Book()
        {
            Stock = 3;
            BookAuthors = new List<BookAuthor>();

            EpubFilePath = null; // Initialize as null
            F2bFilePath = null;  // Initialize as null
            MobiFilePath = null; // Initialize as null
            PdfFilePath = null;
        }

        public int BookId { get; set; }

        // Book title
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; }

        // Publisher of the book
        [Required(ErrorMessage = "Publisher is required.")]
        [StringLength(200, ErrorMessage = "Publisher cannot exceed 200 characters.")]
        public string Publisher { get; set; }

        // Description of the book
        public string Description { get; set; }

        // Year the book was published
        [Required(ErrorMessage = "Year published is required.")]
        [Range(1000, 9999, ErrorMessage = "Year published must be a valid year.")]
        public int YearPublished { get; set; }

        // Price to borrow the book (must be lower than BuyPrice)
        [Range(0, double.MaxValue, ErrorMessage = "Borrow price must be a positive value.")]
        public decimal BorrowPrice { get; set; }

        // Price to buy the book
        [Range(0, double.MaxValue, ErrorMessage = "Buy price must be a positive value.")]
        public decimal BuyPrice { get; set; }

        // Discount price for limited time
        [Range(0, double.MaxValue, ErrorMessage = "Discount price must be a positive value.")]
        public decimal? DiscountPrice { get; set; }

        // The date when the discount ends
        public DateTime? DiscountEndDate { get; set; }

        // The stock available for borrowing (maximum of 3)
        [Range(0, 3, ErrorMessage = "Only 3 copies can be borrowed.")]
        public int Stock { get; set; }

        // Age limit for the book (e.g., 18+, 8+, etc.)
        [StringLength(50, ErrorMessage = "Age limit cannot exceed 50 characters.")]
        public string AgeLimit { get; set; }

        // Category of the book
        [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters.")]
        public string Category { get; set; }

        // Image file (not mapped to the database)
        [NotMapped]
        public IFormFile ImageFile { get; set; }

        // Image URL (stored in the database)
        public string? ImageUrl { get; set; } = "images/BookDefault.png";

        // File paths for different formats (nullable)
        public string? EpubFilePath { get; set; }
        public string? F2bFilePath { get; set; }
        public string? MobiFilePath { get; set; }
        public string? PdfFilePath { get; set; }

        // File upload properties (not mapped to the database)
        [NotMapped]
        public IFormFile EpubFile { get; set; }

        [NotMapped]
        public IFormFile F2bFile { get; set; }

        [NotMapped]
        public IFormFile MobiFile { get; set; }

        [NotMapped]
        public IFormFile PdfFile { get; set; }

        // Formats supported by the book
        public string Formats { get; set; } = "epub,f2b,mobi,PDF";

        // Navigation property for authors of the book (Many-to-many relation with Author)
        public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    }
}