using GeneralClasses;
using Server.Data;
using Server.SQLRepositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.SQLRepositories;

public class BooksEFRepository : IBooksSQLRepository
{
    private LibraryDbContext context;
    public BooksEFRepository(LibraryDbContext context)
    {
            context = context;
    }
    public IEnumerable<Book> GetAll()
    {
        return context.Books;
    }

    public IEnumerable<Book> GetAll(IEnumerable<Predicate<Book>> predicates)
    {
        Predicate<Book> predicate = book => {
            foreach (var predicate in predicates) {
                if (predicate(book) == false)
                    return false;
            }
            return true;
        };
        return context.Books.Where(b => predicate(b));
    }

    public Book? GetById(int id)
    {
       return context.Books.FirstOrDefault(b => b.Id == id);
    }

    public void Update(Book book)
    {
       context.Books.Update(book);
        context.SaveChanges();
    }
}
