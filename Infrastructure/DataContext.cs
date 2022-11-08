using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Author>().Property(a => a.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<Book>().Property(b => b.Id).HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<Author>().HasData(new Author() { Id = Guid.NewGuid(), FullName = "Hüsamettin DÖNÜŞ" });
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
    }
}
