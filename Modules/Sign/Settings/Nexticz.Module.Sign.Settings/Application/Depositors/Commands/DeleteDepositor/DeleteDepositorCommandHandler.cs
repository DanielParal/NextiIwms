using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Application.Depositors.Queries.GetDepositorByCode;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Application.Users.Queries.GetUsersByDepositorCode;
using Nexticz.Module.Sign.Settings.Domain.DepositorAggregate;
using Nexticz.Module.Sign.Settings.Domain.DepositorAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.Depositors.Commands.DeleteDepositor;

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
            logger.LogInformation("Sign - Did not find object {ObjectName} with code: {Code}. Nothing to delete.", nameof(Depositor), request.Code);
            return DepositorErrors.ValidationCodeDoesNotExist;
        }
        
        var users = await sender.Send(new GetUsersByDepositorCodeQuery(depositor.Value.Code), cancellationToken);
        if (users.Length > 0)
        {
            var userNamesString = string.Join(", ", users.Select(x => x.UserName));
            logger.LogInformation("Sign - we cannot delete {ObjectName} with code: {Code} because it is assigned to users: {userNamesString}.",
                nameof(Depositor), depositor.Value.Code, userNamesString);
            return DepositorErrors.ValidationCodeIsAssignedToUsers(userNamesString);
        }
        
        var depositorDeletedEvent = new DepositorDeletedEvent(depositor.Value.Id, request.Code);
        unitOfWork.AppendEvent(depositor.Value.Id, depositorDeletedEvent);
        
        logger.LogInformation("Sign - Object {ObjectName} with code: {Code} deleted.",
            nameof(Depositor), request.Code);
        return Result.Deleted;
    }
}