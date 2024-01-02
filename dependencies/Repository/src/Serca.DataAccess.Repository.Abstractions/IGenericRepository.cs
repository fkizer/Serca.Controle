using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serca.DataAccess.Repository.Abstractions
{
    public interface IGenericRepository<TEntity, TKey> where TEntity : IAggregateRoot
    {
        Task<TEntity?> GetByIdAsync(TKey id);
        Task<IReadOnlyList<TEntity>?> GetAllAsync();
        Task AddAsync(TEntity entity, TKey? id = default);
        Task UpdateAsync(TEntity? entity, TKey? id = default);
        Task DeleteAsync(TEntity entity);
    }
}
