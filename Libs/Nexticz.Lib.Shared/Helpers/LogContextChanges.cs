using Microsoft.EntityFrameworkCore;
using Nexticz.Lib.Shared.DataAccess.EntityFramework;

namespace Nexticz.Lib.Shared.Helpers;

public static class LogContextChanges
{
    public static List<LoggedItem> GetLoggedItems(IBaseUnitOfWork unitOfWork)
    {
        var entries = unitOfWork.GetContext().ChangeTracker
            .Entries()
            .Where(x => x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        var addedEntries = entries
            .Where(x => x.State == EntityState.Added)
            .ToList();
        var modifiedEntries = entries
            .Where(x => x.State == EntityState.Modified)
            .ToList();
        var removedEntries = entries
            .Where(x => x.State == EntityState.Deleted)
            .ToList();

        var logItems = addedEntries
            .Select(addedEntry => new LoggedItem(ContextChangeType.Added, addedEntry.Entity))
            .ToList();
        logItems
            .AddRange(modifiedEntries
                .Select(modifiedEntry => new LoggedItem(ContextChangeType.ChangedTo, modifiedEntry.Entity)));

        logItems
            .AddRange(removedEntries
                .Select(removedEntry => new LoggedItem(ContextChangeType.Removed, removedEntry.Entity)));

        return logItems;
    }
}

public record LoggedItem(ContextChangeType Type, object LoggedObject);

public enum ContextChangeType
{
    Added,
    ChangedFrom,
    ChangedTo,
    Removed
}