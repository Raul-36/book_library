using GeneralClasses;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.SQLRepositories.Base;

public interface IBooksSQLRepository
{
    public IEnumerable<Book> GetAll();
    public void UpdateUserId(int id,Book book);
    public Book? GetById(int id);
}
