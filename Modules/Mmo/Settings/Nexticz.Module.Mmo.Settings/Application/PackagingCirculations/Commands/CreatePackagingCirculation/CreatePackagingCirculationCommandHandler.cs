using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.PackagingCirculations.Queries.GetPackagingCirculationByCode;
using Nexticz.Module.Mmo.Settings.Domain.PackagingCirculationEntity;
using Nexticz.Module.Mmo.Settings.Domain.PackagingCirculationEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.PackagingCirculations.Commands.CreatePackagingCirculation;

internal class CreatePackagingCirculationCommandHandler(
    ILogger<CreatePackagingCirculationCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender) 
    : IRequestHandler<CreatePackagingCirculationCommand, ErrorOr<PackagingCirculation>>
{
    public async Task<ErrorOr<PackagingCirculation>> Handle(CreatePackagingCirculationCommand request, CancellationToken cancellationToken)
    {
        var existingPackageCirculation = await sender.Send(new GetPackagingCirculationByCodeQuery(request.Code), cancellationToken);

        if (existingPackageCirculation.HasValue())
        {
            logger.LogInformation("Object {ObjectName} with code: {Code} already exists. Nothing to create.", nameof(PackagingCirculation), request.Code);
            return PackagingCirculationErrors.ValidationCodeAlreadyExists(request.Code);
        }
        
        var packageCirculation = new PackagingCirculation(request.Code, request.Name);
        var packageCirculationCreatedEvent = new PackagingCirculationCreatedEvent(packageCirculation.Id, packageCirculation.Code, packageCirculation.Name);
        
        unitOfWork.StartStream<PackagingCirculationCreatedEvent, PackagingCirculation>(packageCirculation.Id, packageCirculationCreatedEvent);

        return packageCirculation;
    }
}