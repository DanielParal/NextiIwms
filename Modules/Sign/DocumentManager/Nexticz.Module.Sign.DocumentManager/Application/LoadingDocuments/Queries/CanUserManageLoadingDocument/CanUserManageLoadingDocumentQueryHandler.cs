using MediatR;
using Nexticz.Module.Sign.Settings.Contracts.Depositors.Queries;

namespace Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Queries.CanUserManageLoadingDocument;

internal class CanUserManageLoadingDocumentQueryHandler(
    ISender sender) : IRequestHandler<CanUserManageLoadingDocumentQuery, bool>
{
    public Task<bool> Handle(CanUserManageLoadingDocumentQuery request, CancellationToken cancellationToken)
    {
        var upperDepositorCodes = request.UsersDepositorCodes.Select(dc => dc.ToUpperInvariant()).ToArray();

        return Task.FromResult(upperDepositorCodes.Contains(request.LoadingDocument.DepositorCode));
    }
}