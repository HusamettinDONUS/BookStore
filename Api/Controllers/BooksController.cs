using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Api.DTO;

namespace Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IUnitOfWork<Book> _book;
        private readonly IUnitOfWork<Author> _author;
        public BooksController(IUnitOfWork<Book> book, IUnitOfWork<Author> author)
        {
            _book = book;
            _author = author;
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks()
        {
            var books = await _book.Entity.GetAllAsync();
            var booksDTO = new List<BookDTO>();
            foreach (var book in books)
            {
                booksDTO.Add(BookToDto(book));
            }
            return  Ok(booksDTO);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            if (id == Guid.Empty) return NotFound(); //404
            var book = await _book.Entity.GetByIdAsync(id); //200
            if (book == null) return NotFound(); //404
            return Ok(BookToDto(book));
        }

        [HttpPost]
        public async Task<IActionResult> Create(Book bookReq)
        {
            Book book = new Book
                    {
                        Title = bookReq.Title,
                        Description = bookReq.Description,
                        Author = _author.Entity.GetById(bookReq.Author.Id),
                        ImageUrl = bookReq.ImageUrl
                    };
            await _book.Entity.InsertAsync(book);
            await _book.SaveAsync();
            return CreatedAtAction(nameof(GetBookById), new {id =book.Id }, BookToDto(book));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(Guid id, Book entity)
        {
            if(id != entity.Id) return BadRequest();

            var book = await _book.Entity.GetByIdAsync(id);
            if(book != null) 
            {
                book.Title = entity.Title;
                book.Description = entity.Description;
                book.Author = _author.Entity.GetById(entity.Author.Id);
                book.ImageUrl = entity.ImageUrl;

                _book.Entity.Update(book);
                await _book.SaveAsync();
                return NoContent();
            }  
            else
            {
                return NotFound();
            }            
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            var book = await _book.Entity.GetByIdAsync(id);
            if(book == null) return NotFound();

            _book.Entity.Delete(id);
            await _book.SaveAsync();
            return NoContent();
        }

        private static BookDTO BookToDto(Book b)
        {
            var book = new BookDTO
                {
                    Title = b.Title,
                    Description = b.Description,
                    Author = b.Author
                };
            return book;
        }
    }
}