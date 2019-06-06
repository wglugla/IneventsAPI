using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IUsersEventsRepository : IRepositoryBase<UsersEvents>
    {
        Task<int[]> GetUsersEventsAsync(int userId);
        Task AddMemberAsync(int userId, int eventId);
        Task RemoveMemberAsync(int userId, int eventId);
    }
}
