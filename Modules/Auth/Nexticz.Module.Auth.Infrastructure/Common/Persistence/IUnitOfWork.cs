using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Nexticz.Module.Auth.Application.Common.Interfaces;
using Nexticz.Module.Auth.Infrastructure.AppUserApiKeys.Persistence;
using Nexticz.Module.Auth.Infrastructure.AppUserRefreshTokens.Persistence;

namespace Nexticz.Module.Auth.Infrastructure.Common.Persistence;

public class UnitOfWork(DataContext context) : IUnitOfWork
{
    public IAppUserRefreshTokensRepository AppUserRefreshTokensRepository =>
        new AppUserRefreshTokensRepository(context);

    public IAppUserApiKeysRepository AppUserApiKeysRepository => new AppUserApiKeysRepository(context);

    public async Task<bool> CompleteAsync(CancellationToken token = default)
    {
        var count = await context.SaveChangesAsync(token);
        return count > 0;
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken token = default)
    {
        return await context.Database.BeginTransactionAsync(token);
    }

    public void Add(object item)
    {
        context.Add(item);
    }

    public void Update(object item)
    {
        context.Update(item);
    }

    public void Remove(object item)
    {
        context.Remove(item);
    }

    public void AddRange(IEnumerable<object> items)
    {
        context.AddRange(items);
    }

    public void UpdateRange(IEnumerable<object> items)
    {
        context.UpdateRange(items);
    }

    public void RemoveRange(IEnumerable<object> items)
    {
        context.RemoveRange(items);
    }

    public DbContext GetContext()
    {
        return context;
    }
}