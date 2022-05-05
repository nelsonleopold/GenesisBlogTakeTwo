using System.ComponentModel.DataAnnotations;

namespace GenesisBlogTakeTwo.Models
{
    public class BlogPostComment
    {
        public int Id { get; set; }
        public int BlogPostId { get; set; }

        [Required]
        public string Comment { get; set; } = string.Empty;




        // Navigational Properties
        public virtual BlogPost BlogPost { get; set; } = default!;
    }
}
