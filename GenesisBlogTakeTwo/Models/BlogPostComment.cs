using System.ComponentModel.DataAnnotations;

namespace GenesisBlogTakeTwo.Models
{
    public class BlogPostComment
    {
        public int Id { get; set; }
        public int BlogPostId { get; set; }
        public string AuthorId { get; set; } = Guid.Empty.ToString();

        public DateTime Created { get; set; }


        [Required]
        public string Comment { get; set; } = string.Empty;




        // Navigational Properties
        public virtual BlogPost? BlogPost { get; set; }
        public virtual BlogUser? Author { get; set; }

    }
}
