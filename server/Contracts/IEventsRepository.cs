using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IEventsRepository : IRepositoryBase<Event>
    {
        Task<IEnumerable<object>> GetAllEventsAsync();
        Task<Event> GetEventByIdAsync(int eventId);
        Task CreateEventAsync(Event eventTarget);
        Task DeleteEventAsync(Event eventTarget);
        Task ModifyEventAsync(Event modified);
    }
}
