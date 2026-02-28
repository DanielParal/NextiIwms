using MediatR;
using Nexticz.Module.Sign.Settings.Application.Interfaces;

namespace Nexticz.Module.Sign.Settings.Application.Projections.Commands;

internal class RebuildProjectionCommandHandler(ISettingsUnitOfWork unitOfWork) : IRequestHandler<RebuildProjectionCommand, bool>
{
    public async Task<bool> Handle(RebuildProjectionCommand request, CancellationToken cancellationToken)
    {
        return await unitOfWork.RebuildProjectionAsync(request.ProjectionTypeString, cancellationToken);
    }
}