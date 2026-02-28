using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.PackagingCirculations.Queries.GetPackagingCirculationByCode;
using Nexticz.Module.Mmo.Settings.Domain.PackagingCirculationEntity;
using Nexticz.Module.Mmo.Settings.Domain.PackagingCirculationEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.PackagingCirculations.Commands.UpdatePackagingCirculation;

internal class UpdatePackagingCirculationCommandHandler(
    ILogger<UpdatePackagingCirculationCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender)
    : IRequestHandler<UpdatePackagingCirculationCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdatePackagingCirculationCommand request, CancellationToken cancellationToken)
    {
        var packageCirculation = await sender.Send(new GetPackagingCirculationByCodeQuery(request.Code), cancellationToken);
        if (!packageCirculation.HasValue())
        {
            logger.LogInformation("Did not find object {ObjectName} with code: {Code}. Nothing to update.", nameof(PackagingCirculation), request.Code);
            return PackagingCirculationErrors.CodeDoesNotExist;
        }

        var manufactureUpdatedEvent = new PackagingCirculationNameUpdatedEvent(packageCirculation.Value.Id, request.Name);

        unitOfWork.AppendEvent(packageCirculation.Value.Id, manufactureUpdatedEvent);

        return Result.Updated;
    }
}