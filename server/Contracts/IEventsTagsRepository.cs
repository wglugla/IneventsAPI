using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IEventsTagsRepository : IRepositoryBase<EventsTags>
    {
        Task<int[]> GetEventTagsAsync(int eventId);
        Task<int[]> GetEventsByTag(int tagId);
    }
}
