using GenesisBlogTakeTwo.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GenesisBlogTakeTwo.Models
{
    public class BlogPost
    {
        public int Id { get; set; }

        [Required]
        [StringLength(250, ErrorMessage = "The {0} must be at least {2} characters and no more than {1} characters long.", MinimumLength = 2)]
        public string Title { get; set; } = string.Empty;


        [Required]
        public string Abstract { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime Created { get; set; } = DateTime.UtcNow;

        public DateTime? Updated { get; set; }

        // This property is derived from the title and in some cases will be
        // used instead of primary key
        [Required]
        public string Slug { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }

        public BlogPostState BlogPostState { get; set; }

        // Image properties
        public byte[] ImageData { get; set; } = Array.Empty<byte>();
        public string ImageType { get; set; } = string.Empty;
        [NotMapped]
        public IFormFile? ImageFile { get; set; }


        // Navigational Properties
        public virtual ICollection<BlogPostComment> BlogPostComments { get; set; } = new HashSet<BlogPostComment>();
    }
}
