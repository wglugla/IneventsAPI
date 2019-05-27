using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository
{
    public class UserRepository : RepositoryBase<User>, IUsersRepository
    {
        public UserRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public IEnumerable<object> GetAllUsers()
        {
            return FindAll()
                .Select(p => new {
                    p.Id,
                    p.Username,
                    p.Name,
                    p.Surname,
                    p.Create_time
                });
        }

        public User GetUserById(int userId)
        {
            return FindByCondition(user => user.Id.Equals(userId)).DefaultIfEmpty(new User()).FirstOrDefault();
        }

        public void CreateUser(User user)
        {
            Create(user);
        }

        public void DeleteUser(User user)
        {
            Delete(user);
        }

        public User GetUserByUsername(string username)
        {
            return FindByCondition(user => user.Username == username).DefaultIfEmpty(new User()).FirstOrDefault();
        }
    }
}
