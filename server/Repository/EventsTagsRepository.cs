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
    class EventsTagsRepository : RepositoryBase<EventsTags>, IEventsTagsRepository
    {
        public EventsTagsRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int[]> GetEventTagsAsync(int eventId)
        {
            int[] tagsIds = await FindByCondition(p => p.EventId.Equals(eventId)).Select(p => p.TagId).ToArrayAsync();
            return tagsIds;
        }
    }
}
