using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class UserRepository : RepositoryBase<User>, IUsersRepository
    {
        public UserRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<object>> GetAllUsersAsync()
        {
            return await FindAll()
                .Select(p => new {
                    p.Id,
                    p.Username,
                    p.Name,
                    p.Surname,
                    p.Create_time
                }).ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await FindByCondition(user => user.Id.Equals(userId)).FirstOrDefaultAsync();
        }

        public async Task CreateUserAsync(User user)
        {
            Create(user);
            await SaveAsync();
        }

        public async Task DeleteUserAsync(User user)
        {
            Delete(user);
            await SaveAsync();
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await FindByCondition(user => user.Username == username).FirstOrDefaultAsync();
        }
    }
}
