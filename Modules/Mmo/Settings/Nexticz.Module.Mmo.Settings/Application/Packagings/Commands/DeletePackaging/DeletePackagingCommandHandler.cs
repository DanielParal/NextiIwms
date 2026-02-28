using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitsByPackagingCode;
using Nexticz.Module.Mmo.Settings.Application.Packagings.Queries.GetPackagingByCode;
using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate;
using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate.Events;

namespace Nexticz.Module.Mmo.Settings.Application.Packagings.Commands.DeletePackaging;

internal class DeletePackagingCommandHandler (
    ISender sender, 
    ISettingsUnitOfWork unitOfWork,
    ILogger<DeletePackagingCommandHandler> logger)
    : IRequestHandler<DeletePackagingCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeletePackagingCommand request, CancellationToken cancellationToken)
    {
        var package = await sender.Send(new GetPackagingByCodeQuery(request.Code), cancellationToken);

        if (package.IsError)
        {
            logger.LogInformation("Did not find object {ObjectName} with Code: {Code}. Nothing to delete.", nameof(Packaging), request.Code);
            return package.Errors;
        }
        
        var existingKitsWithPackaging = await sender.Send(new GetKitsByPackagingCodeQuery(request.Code), cancellationToken);
        if (existingKitsWithPackaging.Any())
        {
            var existingKitsWithPackagingComaSeparated = string.Join(", ", existingKitsWithPackaging.Select(x => x.Code));
            logger.LogInformation("Packaging {Code} is still used in the following kits: {Kits}", request.Code, existingKitsWithPackagingComaSeparated);
            return PackagingErrors.ValidationPackagingIsUsedInKits(existingKitsWithPackagingComaSeparated);
        }
        
        var packageDeletedEvent = new PackagingDeletedEvent(package.Value.Id, request.Code);
        unitOfWork.AppendEvent(package.Value.Id, packageDeletedEvent);
        return Result.Deleted;
    }
}