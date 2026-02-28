using ErrorOr;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.Orchestrators.ChangeComment;

internal interface IChangeCommentOrchestrator
{
    Task<ErrorOr<Success>> ChangeCommentAsync(Guid id, string comment, CancellationToken cancellationToken);
}