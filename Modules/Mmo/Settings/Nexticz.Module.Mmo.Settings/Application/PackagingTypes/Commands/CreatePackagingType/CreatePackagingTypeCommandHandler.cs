using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.PackagingTypes.Queries.GetPackagingTypeByCode;
using Nexticz.Module.Mmo.Settings.Domain.PackagingTypeEntity;
using Nexticz.Module.Mmo.Settings.Domain.PackagingTypeEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.PackagingTypes.Commands.CreatePackagingType;

internal class CreatePackagingTypeCommandHandler(
    ILogger<CreatePackagingTypeCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender) 
    : IRequestHandler<CreatePackagingTypeCommand, ErrorOr<PackagingType>>
{
    public async Task<ErrorOr<PackagingType>> Handle(CreatePackagingTypeCommand request, CancellationToken cancellationToken)
    {
        var existingPackagingType = await sender.Send(new GetPackagingTypeByCodeQuery(request.Code), cancellationToken);

        if (existingPackagingType.HasValue())
        {
            logger.LogInformation("Object {ObjectName} with code: {Code} already exists. Nothing to create.", nameof(PackagingType), request.Code);
            return PackagingTypeErrors.ValidationCodeAlreadyExists(request.Code);
        }
        
        var packagingType = new PackagingType(request.Code, request.Name);
        var packageTypeCreatedEvent = new PackagingTypeCreatedEvent(packagingType.Id, packagingType.Code, packagingType.Name);
        
        unitOfWork.StartStream<PackagingTypeCreatedEvent, PackagingType>(packagingType.Id, packageTypeCreatedEvent);

        return packagingType;
    }
}