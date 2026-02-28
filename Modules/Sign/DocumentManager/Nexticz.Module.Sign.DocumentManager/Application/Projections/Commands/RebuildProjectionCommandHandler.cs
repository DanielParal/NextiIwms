using MediatR;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;

namespace Nexticz.Module.Sign.DocumentManager.Application.Projections.Commands;

internal class RebuildProjectionCommandHandler(IDocumentManagerUnitOfWork unitOfWork) : IRequestHandler<RebuildProjectionCommand, bool>
{
    public async Task<bool> Handle(RebuildProjectionCommand request, CancellationToken cancellationToken)
    {
        return await unitOfWork.RebuildProjectionAsync(request.ProjectionTypeString, cancellationToken);
    }
}