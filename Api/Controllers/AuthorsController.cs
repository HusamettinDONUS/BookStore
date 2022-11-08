using System;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly IUnitOfWork<Author> _author;
        public AuthorsController(IUnitOfWork<Author> author)
        {
            _author = author;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuthors()
        {
            var authors = await _author.Entity.GetAllAsync();
            return  Ok(authors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthorById(Guid id)
        {
            if (id == Guid.Empty) return NotFound(); //404

            var author = await _author.Entity.GetByIdAsync(id); //200
            if (author == null) return NotFound(); //404

            return Ok(author);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Author authorReq)
        {
            Author author = new Author { FullName = authorReq.FullName };
            await _author.Entity.InsertAsync(author);
            await _author.SaveAsync();
            return CreatedAtAction(nameof(GetAuthorById), new {id =author.Id }, author);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(Guid id, Author entity)
        {
            if(id != entity.Id) return BadRequest();
            var author = await _author.Entity.GetByIdAsync(id);
            if(author != null) 
            {
                author.FullName = entity.FullName;
                _author.Entity.Update(author);
                await _author.SaveAsync();
                return NoContent();
            }  
            else return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(Guid id)
        {
            var author = await _author.Entity.GetByIdAsync(id);
            if(author == null) return NotFound();
            
            _author.Entity.Delete(id);
            await _author.SaveAsync();
            return NoContent();
        }
    }
}