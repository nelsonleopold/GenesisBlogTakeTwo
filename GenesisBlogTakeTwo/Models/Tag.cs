using System.ComponentModel.DataAnnotations;

namespace GenesisBlogTakeTwo.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;

        public string? Description { get; set; }


        // Navigational properties

        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new HashSet<BlogPost>();

    }
}
