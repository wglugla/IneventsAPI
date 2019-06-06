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
    class UsersTagsRepository : RepositoryBase<UsersTags>, IUsersTagsRepository
    {
        public UsersTagsRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int[]> GetUserTagsAsync(int userId)
        {
            int[] tagsIds = await FindByCondition(p => p.UserId.Equals(userId)).Select(p => p.TagId).ToArrayAsync();
            return tagsIds;
        }
    }
}
