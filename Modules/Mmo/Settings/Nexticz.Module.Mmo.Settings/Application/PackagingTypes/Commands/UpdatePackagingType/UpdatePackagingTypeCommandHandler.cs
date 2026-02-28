using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.PackagingTypes.Queries.GetPackagingTypeByCode;
using Nexticz.Module.Mmo.Settings.Domain.PackagingTypeEntity;
using Nexticz.Module.Mmo.Settings.Domain.PackagingTypeEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.PackagingTypes.Commands.UpdatePackagingType;

internal class UpdatePackagingTypeCommandHandler (
    ILogger<UpdatePackagingTypeCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender)
    : IRequestHandler<UpdatePackagingTypeCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdatePackagingTypeCommand request, CancellationToken cancellationToken)
    {
        var packagingType = await sender.Send(new GetPackagingTypeByCodeQuery(request.Code), cancellationToken);

        if (packagingType.IsError)
        {
            logger.LogWarning("Did not find object {ObjectName} with code: {Code}. Nothing to update.", nameof(PackagingType), request.Code);
            return packagingType.Errors;
        }
        
        var packageTypeUpdatedEvent = new PackagingTypeNameUpdatedEvent(packagingType.Value.Id, request.Name);
        unitOfWork.AppendEvent(packagingType.Value.Id, packageTypeUpdatedEvent);
        return Result.Updated;
    }
}