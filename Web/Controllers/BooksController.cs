using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Controllers
{
    public class BooksController : Controller
    {
        private readonly IUnitOfWork<Book> _book;
        private readonly IUnitOfWork<Author> _author;
        public BooksController(IUnitOfWork<Book> book, IUnitOfWork<Author> author)
        {
            _book = book;
            _author = author;
        }

        // GET: BooksController
        public ActionResult Index()
        {
            var books = _book.Entity.GetAll();
            return View(books);
        }

        // GET: BooksController/Details/5
        public ActionResult Details(Guid id)
        {
            if (id == Guid.Empty) return NotFound();

            var book = _book.Entity.GetById(id);
            if (book == null) return NotFound();

            return View(book);
        }

        // GET: BooksController/Create
        public ActionResult Create()
        {
            return View(GetAllAuthors());
        }

        // POST: BooksController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BookAuthorVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.AuthorId == Guid.Empty)
                     {
                         ViewBag.Message = "Please select an author from the list!";
                         return View(GetAllAuthors());
                     }
                    
                    Book book = new Book();
                    book.Title = model.Title;
                    book.Description = model.Description;
                    book.Author = _author.Entity.GetById(model.AuthorId);

                    if(model.File != null)
                    {                   
                        var randomName = string.Format($"{DateTime.Now.Ticks}{Path.GetExtension(model.File.FileName)}");
                        book.ImageUrl = randomName;
                        var newPath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\uploads", randomName);
                        using(var stream = new FileStream(newPath, FileMode.Create))
                        {
                            model.File.CopyTo(stream);
                        }
                    }
                    else
                    {
                        book.ImageUrl = "no-image-icon.png";
                    }

                    _book.Entity.Insert(book);
                    _book.Save();
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View();
                }
            }
            ModelState.AddModelError("", "You have to fill all required fields!");
            return View(GetAllAuthors());
        }

        // GET: BooksController/Edit/5
        public ActionResult Edit(Guid id)
        {
            var book = _book.Entity.GetById(id);

            var vm = new BookAuthorVM
            {
                BookId = book.Id,
                Title = book.Title,
                Description = book.Description,
                AuthorId = book.Author.Id,
                ImageUrl = book.ImageUrl,
                Authors = GetAllAuthors().Authors
            };
            return View(vm);
        }

        // POST: BooksController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Guid id, BookAuthorVM model)
        {
            if (id != model.BookId) return NotFound();
            try
            {
                Book book = new Book();
                book.Id = model.BookId;
                book.Title = model.Title;
                book.Description = model.Description;
                book.Author = _author.Entity.GetById(model.AuthorId);

                if(model.File != null)
                {                   
                    var randomName = string.Format($"{DateTime.Now.Ticks}{Path.GetExtension(model.File.FileName)}");
                    book.ImageUrl = randomName;
                    var newPath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\uploads",randomName);
                    using(var stream = new FileStream(newPath, FileMode.Create))
                    {
                        model.File.CopyTo(stream);
                    }

                    if(model.ImageUrl != "no-image-icon.png")
                    {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\uploads",model.ImageUrl);                    
                    System.IO.File.Delete(oldPath);
                    }
                }
                else
                {
                    book.ImageUrl = model.ImageUrl;
                }
                
                _book.Entity.Update(book);
                _book.Save();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View();
            }
        }

        // GET: BooksController/Delete/5
        public ActionResult Delete(Guid id)
        {
            if (id == null) return NotFound();

            var book = _book.Entity.GetById(id);
            if (book == null) return NotFound();

            return View(book);
        }

        // POST: BooksController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmDelete(Guid id)
        {
            try
            {
                var book = _book.Entity.GetById(id);
                if(book.ImageUrl != "no-image-icon.png")
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\uploads",book.ImageUrl);                    
                    System.IO.File.Delete(path);
                }
                
                _book.Entity.Delete(id);
                _book.Save();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Search(string term)
        {
            var result = _book.Entity.Search(term);
            return View("Index", result);
        }

        BookAuthorVM GetAllAuthors()
        {
            var authors = _author.Entity.GetAll().ToList();
            authors.Insert(0, new Author { Id = Guid.Empty, FullName = "...Please select an author..." });

            var vmodel = new BookAuthorVM
            {
                Authors = authors
            };
            return vmodel;
        }

        public async Task<IActionResult> GetBooksFromstApi()
        {
            var books = new List<Book>();

            using(var httpClient = new HttpClient())
            {
                using(var response = await httpClient.GetAsync("https://localhost:4200/api/books"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    books = JsonConvert.DeserializeObject<List<Book>>(apiResponse);
                }
            }
            return View(books);
        }
    }
}
