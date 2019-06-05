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
    class UsersEventsRepository : RepositoryBase<UsersEvents>, IUsersEventsRepository
    {
        public UsersEventsRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int[]> GetUsersEventsAsync(int userId)
        {
            int[] ids = await FindByCondition(p => p.UserId.Equals(userId)).Select(p => p.EventId).ToArrayAsync();
            return ids;
        }
    }
}
