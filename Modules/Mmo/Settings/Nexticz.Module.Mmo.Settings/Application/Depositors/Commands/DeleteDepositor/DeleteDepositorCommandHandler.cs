using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Application.Depositors.Queries.GetDepositorByCode;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitsByDepositorCode;
using Nexticz.Module.Mmo.Settings.Application.Packagings.Queries.GetPackagingsByDepositorCode;
using Nexticz.Module.Mmo.Settings.Domain.DepositorEntity;
using Nexticz.Module.Mmo.Settings.Domain.DepositorEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.Depositors.Commands.DeleteDepositor;

internal class DeleteDepositorCommandHandler(
    ILogger<DeleteDepositorCommandHandler> logger,
    ISender sender,
    ISettingsUnitOfWork unitOfWork
    ) 
    : IRequestHandler<DeleteDepositorCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteDepositorCommand request, CancellationToken cancellationToken)
    {
        var depositor = await sender.Send(new GetDepositorByCodeQuery(request.Code), cancellationToken);

        if (depositor.IsError)
        {
            logger.LogInformation("Did not find object {ObjectName} with code: {Code}. Nothing to delete.", nameof(Depositor), request.Code);
            return depositor.Errors;
        }
        
        var existingKitsWithDepositorCode = await sender.Send(new GetKitsByDepositorCodeQuery(request.Code), cancellationToken);
        var existingPackagingsWithDepositorCode = await sender.Send(new GetPackagingsByDepositorCodeQuery(request.Code), cancellationToken);
        
        if (existingKitsWithDepositorCode.Any() || existingPackagingsWithDepositorCode.Any())
        {
            var kitCodes = string.Join(", ", existingKitsWithDepositorCode.Select(x => x.Code));
            var packagingCodes = string.Join(", ", existingPackagingsWithDepositorCode.Select(x => x.Code));
            logger.LogInformation(
                "{ObjectName} is still used either inside kits with codes: {KitCodes} or inside packagings with codes: {PackagingCodes}", 
                nameof(Depositor), kitCodes, packagingCodes);
            return DepositorErrors.ValidationCodeIsStillUsedEitherInKitsOrInPackagings(kitCodes, packagingCodes);
        }
        
        var depositorDeletedEvent = new DepositorDeletedEvent(depositor.Value.Id, request.Code);
        unitOfWork.AppendEvent(depositor.Value.Id, depositorDeletedEvent);
        return Result.Deleted;
    }
}