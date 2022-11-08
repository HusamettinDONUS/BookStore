using Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class BookAuthorVM
    {
        public Guid BookId { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 5)]
        public string Title { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 9)]
        public string Description { get; set; }
        public Guid AuthorId { get; set; }
        public string ImageUrl { get; set; }
        public List<Author> Authors { get; set; }
        public IFormFile File { get; set; }
    }
}
