using System;
using System.Collections.Generic;
using System.Text;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions options) : base(options) { }

        // model as table in database
        public DbSet<User> User { get; set; }

        public DbSet<Event> Event { get; set; }

        public DbSet<Tag> Tag { get; set; }

        public DbSet<EventsTags> EventsTags { get; set; }

        public DbSet<UsersEvents> UsersEvents { get; set; }

    }
}
