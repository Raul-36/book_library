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
        this.context = context;
    }


    public IEnumerable<Book> GetAll()
    {
        if (context.Books.Count() > 0)
        {
            return context.Books.ToArray();
        }
        return Enumerable.Empty<Book>();

    }

    public Book? GetById(int id)
    {
        return context.Books.FirstOrDefault(b => b.Id == id);
    }

    public async void UpdateUserId(int id, Book book)
    {
        /*if (book.Id == default)
            book.Id = id;

        this.context.Books.Update(book);*/
        var existingBook = new Book { Id = id };
        context.Books.Attach(existingBook);
        existingBook.UserId = book.UserId;
        await this.context.SaveChangesAsync();
    }
}
