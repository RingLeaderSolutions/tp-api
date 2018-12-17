using System;
using System.Threading.Tasks;

namespace Theta.Paltform.Order.Read.Service.Framework
{
    public interface IAggregateRepository
    {
        Task<T> GetAsync<T>(Guid id) where T : IAggregateRoot;
        Task Save(IAggregateRoot aggregateRoot);
    }
}