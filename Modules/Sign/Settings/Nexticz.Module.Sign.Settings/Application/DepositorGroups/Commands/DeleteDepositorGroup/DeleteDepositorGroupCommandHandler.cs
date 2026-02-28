using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Application.DepositorGroups.Queries.GetDepositorGroupByCode;
using Nexticz.Module.Sign.Settings.Application.Depositors.Queries.GetDepositorsByDepositorGroupCode;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Application.Users.Queries.GetUsersByDepositorGroupCode;
using Nexticz.Module.Sign.Settings.Domain.DepositorGroupAggregate;
using Nexticz.Module.Sign.Settings.Domain.DepositorGroupAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.DepositorGroups.Commands.DeleteDepositorGroup;

internal class DeleteDepositorGroupCommandHandler(
    ILogger<DeleteDepositorGroupCommandHandler> logger,
    ISender sender,
    ISettingsUnitOfWork unitOfWork
) 
    : IRequestHandler<DeleteDepositorGroupCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteDepositorGroupCommand request, CancellationToken cancellationToken)
    {
        var depositorGroup = await sender.Send(new GetDepositorGroupByCodeQuery(request.Code), cancellationToken);

        if (depositorGroup.IsError)
        {
            logger.LogInformation("Sign - Did not find object {ObjectName} with code: {Code}. Nothing to delete.", nameof(DepositorGroup), request.Code);
            return DepositorGroupErrors.ValidationCodeDoesNotExist;
        }
        
        var depositors = await sender.Send(new GetDepositorsByDepositorGroupCodeQuery(depositorGroup.Value.Code), cancellationToken);
        if (depositors.Length > 0)
        {
            var depositorCodes = string.Join(", ", depositors.Select(x => x.Code));
            logger.LogInformation("Sign - we cannot delete {ObjectName} with code: {Code} because it is used in depositors: {depositorCodes}.",
                nameof(DepositorGroup), depositorGroup.Value.Code, depositorCodes);
            return DepositorGroupErrors.ValidationCodeIsUsedInDepositors(depositorCodes);
        }
        
        var users = await sender.Send(new GetUsersByDepositorGroupCodeQuery(depositorGroup.Value.Code), cancellationToken);
        if (users.Length > 0)
        {
            var userNamesString = string.Join(", ", users.Select(x => x.UserName));
            logger.LogInformation("Sign - we cannot delete {ObjectName} with code: {Code} because it is assigned to users: {userNamesString}.",
                nameof(DepositorGroup), depositorGroup.Value.Code, userNamesString);
            return DepositorGroupErrors.ValidationCodeIsAssignedToUsers(userNamesString);
        }
        
        var depositorGroupDeletedEvent = new DepositorGroupDeletedEvent(depositorGroup.Value.Id, request.Code);
        unitOfWork.AppendEvent(depositorGroup.Value.Id, depositorGroupDeletedEvent);
        
        logger.LogInformation("Sign - Object {ObjectName} with code: {Code} deleted.",
            nameof(DepositorGroup), request.Code);
        return Result.Deleted;
    }
}