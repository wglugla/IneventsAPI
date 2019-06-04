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


        public Task CreateEventAsync(Event eventTarget)
        {
            throw new NotImplementedException();
        }

        public Task DeleteEventAsync(Event ueventTarget)
        {
            throw new NotImplementedException();
        }

        

        public Task<User> GetEventByIdAsync(int eventId)
        {
            throw new NotImplementedException();
        }
    }
}
