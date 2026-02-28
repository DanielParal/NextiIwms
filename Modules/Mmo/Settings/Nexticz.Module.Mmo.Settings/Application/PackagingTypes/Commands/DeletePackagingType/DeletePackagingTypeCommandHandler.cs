using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.Packagings.Queries.GetPackagingsByPackagingTypeCode;
using Nexticz.Module.Mmo.Settings.Application.PackagingTypes.Queries.GetPackagingTypeByCode;
using Nexticz.Module.Mmo.Settings.Domain.PackagingTypeEntity;
using Nexticz.Module.Mmo.Settings.Domain.PackagingTypeEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.PackagingTypes.Commands.DeletePackagingType;

internal class DeletePackagingTypeCommandHandler (
    ILogger<DeletePackagingTypeCommandHandler> logger,
    ISender sender,
    ISettingsUnitOfWork unitOfWork)
    : IRequestHandler<DeletePackagingTypeCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeletePackagingTypeCommand request, CancellationToken cancellationToken)
    {
        var packagingType = await sender.Send(new GetPackagingTypeByCodeQuery(request.Code), cancellationToken);

        if (packagingType.IsError)
        {
            logger.LogInformation("Did not find object {ObjectName} with code: {Code}. Nothing to delete.", nameof(PackagingType), request.Code);
            return packagingType.Errors;
        }
        
        var existingPackagings = await sender.Send(new GetPackagingsByPackagingTypeCodeQuery(request.Code), cancellationToken);

        if (existingPackagings.Any())
        {
            var codes = string.Join(", ", existingPackagings.Select(x => x.Code));
            logger.LogInformation("{ObjectName} is still used inside packagings with codes: {Codes}", nameof(PackagingType), codes);
            return PackagingTypeErrors.ValidationCodeIsStillUsedInPackagings(codes);
        }
        
        var packageTypeDeletedEvent = new PackagingTypeDeletedEvent(packagingType.Value.Id, request.Code);
        unitOfWork.AppendEvent(packagingType.Value.Id, packageTypeDeletedEvent);
        return Result.Deleted;
    }
}