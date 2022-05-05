using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GenesisBlogTakeTwo.Models
{
    public class BlogUser : IdentityUser
    {
        [Required]
        [Display(Name = "First Name")]
        [StringLength(40, ErrorMessage = "The {0} must be between {2} characters and {1} characters long.", MinimumLength = 2)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(40, ErrorMessage = "The {0} must be between {2} characters and {1} characters long.", MinimumLength = 2)]
        public string LastName { get; set; } = string.Empty;

        public string? NickName { get; set; }
    }
}
