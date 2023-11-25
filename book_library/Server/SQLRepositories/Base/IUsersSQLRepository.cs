using GeneralClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.SQLRepositories.Base
{
    public interface IUsersSQLRepository
    {
        public User Create(User user);
        public void UpdateBookId(int id, User user);
        public void Delete(int id);
        public User? GetById(int id);
    }
}
