using Contracts;
using Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private RepositoryContext _repoContext;
        private IUsersRepository _user;
        private IEventsRepository _event;
        private ITagsRepository _tag;
        private IEventsTagsRepository _eventstags;
        private IUsersEventsRepository _usersevents;

        public IUsersRepository User {
            get
            {
                if (_user == null)
                {
                    _user = new UserRepository(_repoContext);
                }
                return _user;
            }   
        }

        public IEventsRepository Event
        {
            get
            {
                if (_event == null)
                {
                    _event = new EventsRepository(_repoContext);
                }
                return _event;
            }
        }

        public ITagsRepository Tag
        {
            get
            {
                if (_tag == null)
                {
                    _tag = new TagsRepository(_repoContext);
                }
                return _tag;
            }
        }

        public IEventsTagsRepository EventsTags
        {
            get
            {
                if (_eventstags == null)
                {
                    _eventstags = new EventsTagsRepository(_repoContext);
                }
                return _eventstags;
            }
        }

        public IUsersEventsRepository UsersEvents
        {
            get
            {
                if (_usersevents == null)
                {
                    _usersevents = new UsersEventsRepository(_repoContext);
                }
                return _usersevents;
            }
        }


        public RepositoryWrapper(RepositoryContext repositoryContext)
        {
            _repoContext = repositoryContext;
        }

        public void Save()
        {
            _repoContext.SaveChanges();
        }
    }
}
