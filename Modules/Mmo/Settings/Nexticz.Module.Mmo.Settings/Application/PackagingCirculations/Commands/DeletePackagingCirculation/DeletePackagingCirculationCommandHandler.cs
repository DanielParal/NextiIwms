using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.PackagingCirculations.Queries.GetPackagingCirculationByCode;
using Nexticz.Module.Mmo.Settings.Application.Packagings.Queries.GetPackagingsByPackagingCirculationCode;
using Nexticz.Module.Mmo.Settings.Domain.PackagingCirculationEntity;
using Nexticz.Module.Mmo.Settings.Domain.PackagingCirculationEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.PackagingCirculations.Commands.DeletePackagingCirculation;

internal class DeletePackagingCirculationCommandHandler (
    ILogger<DeletePackagingCirculationCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender
    ) : IRequestHandler<DeletePackagingCirculationCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeletePackagingCirculationCommand request, CancellationToken cancellationToken)
    {
        var packageCirculation = await sender.Send(new GetPackagingCirculationByCodeQuery(request.Code), cancellationToken);

        if (packageCirculation.IsError)
        {
            logger.LogInformation("Did not find object {ObjectName} with code: {Code}. Nothing to delete.", nameof(PackagingCirculation), request.Code);
            return packageCirculation.Errors;
        }
        
        var existingPackagings = await sender.Send(new GetPackagingsByPackagingCirculationCodeQuery(request.Code), cancellationToken);

        if (existingPackagings.Any())
        {
            var codes = string.Join(", ", existingPackagings.Select(x => x.Code));
            logger.LogInformation("{ObjectName} is still used inside packagings with codes: {Codes}", nameof(PackagingCirculation), codes);
            return PackagingCirculationErrors.ValidationCodeIsStillUsedInPackagings(codes);
        }

        var packageCirculationDeletedEvent = new PackagingCirculationDeletedEvent(packageCirculation.Value.Id, request.Code);

        unitOfWork.AppendEvent(packageCirculation.Value.Id, packageCirculationDeletedEvent);

        return Result.Deleted;
    }
}