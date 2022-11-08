using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Book : EntityBase
    {
        [Required]
        [StringLength(10, MinimumLength = 3)]
        public string Title { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string Description { get; set; }
        public Author Author { get; set; }
        public string ImageUrl { get; set; }
    }
}
