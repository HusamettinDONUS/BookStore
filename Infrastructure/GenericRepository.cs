using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DataContext _context;
        private DbSet<T> table = null;
        private DbSet<Book> books = null;
        public GenericRepository(DataContext context)
        {
            _context = context;
            table = _context.Set<T>();
            books = _context.Set<Book>();
        }

        public void Delete(Guid id)
        {
            T existing = GetById(id);
            table.Remove(existing);
        }

        public IEnumerable<T> GetAll()
        {
            if (typeof(T) == typeof(Book))
            {
                books.Include(b => b.Author).ToList();
            }
            return table.ToList();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            if (typeof(T) == typeof(Book))
            { 
                books.Include(b => b.Author).ToList();
            }
            return await table.ToListAsync();   
        }

        public T GetById(Guid id)
        {
            if (typeof(T) == typeof(Book))
            {
                var book = books.Include(b => b.Author).SingleOrDefault(b => b.Id == id);
            }
            return table.Find(id);
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            if (typeof(T) == typeof(Book))
            {
                var book = books.Include(b => b.Author).SingleOrDefault(b => b.Id == id);
            }
            return await table.FindAsync(id);
        }

        public void Insert(T entity)
        {
            table.Add(entity);
        }

        public async Task InsertAsync(T entity)
        {
            await table.AddAsync(entity);
        }

        public List<Book> Search(string term)
        {
            var result = books.Include(b => b.Author).Where(
                b => b.Title.ToLower().Contains(term.ToLower())
                || b.Description.ToLower().Contains(term.ToLower())
                || b.Author.FullName.ToLower().Contains(term.ToLower())).ToList();

            return result;
        }

        public void Update(T entity)
        {
            table.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
