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
    public class TagsRepository : RepositoryBase<Tag>, ITagsRepository
    {
        public TagsRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<Tag>> GetAllTagsAsync()
        {
            return await FindAll().ToListAsync();
        }

        public async Task<Tag> GetTagByIdAsync(int tagId)
        {
            return await FindByCondition(tag => tag.Id.Equals(tagId)).FirstOrDefaultAsync();
        }
    }
}
