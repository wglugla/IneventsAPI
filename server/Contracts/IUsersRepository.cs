using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    // public interface for Users Repository
    public interface IUsersRepository : IRepositoryBase<User>
    {
        Task<IEnumerable<object>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int userId);
        Task CreateUserAsync(User user);
        Task DeleteUserAsync(User user);
        Task<User> GetUserByUsernameAsync(string username);
    }   
}
