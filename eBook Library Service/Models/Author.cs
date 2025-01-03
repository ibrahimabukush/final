using System.ComponentModel.DataAnnotations;

namespace eBook_Library_Service.Models
{
    public class Author
    {
        public int AuthorId { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    }
}
