using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server.Helpers
{
    public class EventDetails
    {
        public int Id { get; set; }

        public int OwnerId { get; set; }

        public string Title { get; set; }

        public DateTime Date { get; set; }

        public string Place { get; set; }

        public string Description { get; set; }

        public string[] Tags { get; set; }

        public EventDetails(Event ev, string[] tags)
        {
            Id = ev.Id;
            OwnerId = ev.OwnerId;
            Title = ev.Title;
            Date = ev.Date;
            Place = ev.Place;
            Description = ev.Description;
            Tags = tags;
        }
    }
}
