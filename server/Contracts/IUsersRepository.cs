using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    // public interface for Users Repository
    public interface IUsersRepository : IRepositoryBase<User>
    {
        IEnumerable<object> GetAllUsers();
        User GetUserById(int userId);
        void CreateUser(User user);
        void DeleteUser(User user);
        User GetUserByUsername(string username);
    }
}
