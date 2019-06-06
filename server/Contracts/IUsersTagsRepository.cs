using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IUsersTagsRepository : IRepositoryBase<UsersTags>
    {
        Task<int[]> GetUserTagsAsync(int userId);
    }
}
