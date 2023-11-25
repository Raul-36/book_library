using GeneralClasses;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.SQLRepositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.SQLRepositories;
public class UsersEFRepository : IUsersSQLRepository
{
    private LibraryDbContext context;
    public UsersEFRepository(LibraryDbContext context)
    {
        this.context = context;
    }
    public User Create(User user)
    {
       context.Users.Add(user);
        context.SaveChanges();
        return user;
    }

    public void Delete(int id)
    {
        User userToDelete = context.Users.FirstOrDefault(u => u.Id == id);

        if (userToDelete != null) { 
            context.Users.Remove(userToDelete);
            context.SaveChanges();
        }
    }

    public User? GetById(int id)
    {
       return context.Users.AsNoTracking().FirstOrDefault(u => id == u.Id);
    }

    public void UpdateBookId(int id, User user)
    {
        if (user.Id == default)
            user.Id = id;

        this.context.Users.Update(user);
        /*var existingUser = new User { Id = id };
        context.Users.Attach(existingUser); 
        existingUser.BookId = user.BookId; */
        this.context.SaveChanges();
    }
}
