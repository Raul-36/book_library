using GeneralClasses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Data;
public class LibraryDbContext : DbContext
{
    private const string connectionString = "Server=localhost;Database=LibraryDb;TrustServerCertificate=True;Trusted_Connection=True;";
    public DbSet<User> Users { get; set; }
   public DbSet<Book> Books { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(connectionString);

        base.OnConfiguring(optionsBuilder);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<User>().Property(u => u.Name).IsRequired();
        modelBuilder.Entity<User>().Property(u => u.Password).IsRequired();
      

        modelBuilder.Entity<Book>().HasKey(b => b.Id);
        modelBuilder.Entity<Book>().Property(b => b.Name).IsRequired();
        modelBuilder.Entity<Book>().Property(b => b.Author).IsRequired();
      
    }
    
}
