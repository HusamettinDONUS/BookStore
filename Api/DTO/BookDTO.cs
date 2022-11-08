using Core.Entities;

namespace Api.DTO
{
    public class BookDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Author Author { get; set; }
    }
}