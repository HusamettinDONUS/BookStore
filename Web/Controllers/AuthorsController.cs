using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Web.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly IUnitOfWork<Author> _author;
        public AuthorsController(IUnitOfWork<Author> author)
        {
            _author = author;
        }

        public ActionResult Index()
        {
            var authors = _author.Entity.GetAll();
            return View(authors);
        }

        public ActionResult Details(Guid id)
        {
            if (id == Guid.Empty) return NotFound();

            var author = _author.Entity.GetById(id);
            if (author == null) return NotFound();

            return View(author);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Author author)
        {
            try
            {
                _author.Entity.Insert(author);
                _author.Save();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(Guid id)
        {
            var author = _author.Entity.GetById(id);
            return View(author);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Guid id, Author author)
        {
            try
            {
                _author.Entity.Update(author);
                _author.Save();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delete(Guid id)
        {
            if (id == null) return NotFound();

            var author = _author.Entity.GetById(id);
            if (author == null) return NotFound();
            
            return View(author);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmDelete(Guid id)
        {
            try
            {
                _author.Entity.Delete(id);
                _author.Save();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
