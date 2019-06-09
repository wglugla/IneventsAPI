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
    public class EventsRepository : RepositoryBase<Event>, IEventsRepository
    {
        public EventsRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<object>> GetAllEventsAsync()
        {
            return await FindAll()
            .Select(p => new {
                p.Id,
                p.OwnerId,
                p.Title,
                p.Place,
                p.Date,
                p.Description
            }).ToListAsync();
        }

        public async Task<Event> GetEventByIdAsync(int eventId)
        {
            return await FindByCondition(p => p.Id.Equals(eventId)).FirstOrDefaultAsync();
        }


        public async Task CreateEventAsync(Event eventTarget)
        {
            Create(eventTarget);
            await SaveAsync();
        }

        public async Task DeleteEventAsync(Event eventTarget)
        {
            Delete(eventTarget);
            await SaveAsync();
        }

        public async Task ModifyEventAsync(Event modified)
        {
            Update(modified);
            await SaveAsync();
        }

    }
}
