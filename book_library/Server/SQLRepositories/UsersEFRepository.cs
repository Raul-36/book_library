using GeneralClasses;
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
       return context.Users.FirstOrDefault(u => id == u.Id);
    }

    public void Update(User user)
    {
       context.Update(user);
        context.SaveChanges();
    }
}
