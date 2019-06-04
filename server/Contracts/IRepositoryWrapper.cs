using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public interface IRepositoryWrapper
    {
        IUsersRepository User { get; }
        IEventsRepository Event { get; }
        ITagsRepository Tag { get; }
        void Save();
    }
}
