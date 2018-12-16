using System;
using System.Threading.Tasks;

namespace Theta.Platform.Order.Management.Service.Framework
{
    public interface IAggregateRepository
    {
        Task<T> GetAsync<T>(Guid id) where T : IAggregateRoot;
        Task Save(IAggregateRoot aggregateRoot);
    }
}