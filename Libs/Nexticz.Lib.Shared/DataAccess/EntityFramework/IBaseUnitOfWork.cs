using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Nexticz.Lib.Shared.DataAccess.EntityFramework;

public interface IBaseUnitOfWork
{
    Task<bool> CompleteAsync(CancellationToken token = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken token = default);
    void Add(object item);
    void Update(object item);
    void Remove(object item);
    void AddRange(IEnumerable<object> items);
    void UpdateRange(IEnumerable<object> items);
    void RemoveRange(IEnumerable<object> items);
    DbContext GetContext();
}