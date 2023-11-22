using GeneralClasses;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.SQLRepositories.Base;

public interface IBookssSQLRepository
{
    public IEnumerable<Book> GetAll();
    public IEnumerable<Book> GetAll(IEnumerable<Predicate<Book>> predicates);
    public void Update(Book book);
    public Book GetById(int id);
}
