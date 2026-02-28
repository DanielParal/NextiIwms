using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Application.Printers.Queries.GetPrintersByCodes;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Sign.Settings.Application.DepositorGroups.Queries.GetDepositorGroupsByCodes;
using Nexticz.Module.Sign.Settings.Application.Depositors.Queries.GetDepositorsByCodes;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Application.SigningDevices.Queries.GetSigningDevicesByCodes;
using Nexticz.Module.Sign.Settings.Application.Users.Queries.GetUserByUserName;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.Users.Commands.UpdateUser;

internal class UpdateUserCommandHandler(
    ISender sender,
    ILogger<UpdateUserCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork)
    : IRequestHandler<UpdateUserCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await sender.Send(new GetUserByUserNameQuery(request.UserName), cancellationToken);

        if (!user.HasValue())
        {
            logger.LogWarning("Sign - Object {ObjectName} with id: {UserName} does not exist. Nothing to update.", 
                nameof(User), request.UserName);
            return UserErrors.ValidationUserNameDoesNotExist;
        }
        
        var validationResult = 
            await ValidateAsync(request.DepositorCodes, request.DepositorGroupCodes, 
                request.SigningDeviceCodes, cancellationToken);
        
        if (validationResult.IsError)
            return validationResult.Errors;
        
        var userUpdatedEvent = 
            new UserUpdatedEvent(user.Value.Id, user.Value.UserName, validationResult.Value.DepositorCodes,
                validationResult.Value.DepositorGroupCodes, validationResult.Value.SigningDeviceCodes);
        unitOfWork.AppendEvent(user.Value.Id, userUpdatedEvent);

        return Result.Updated;
    }
    
    private async Task<ErrorOr<ValidationResult>> ValidateAsync(
        string[] depositorCodes, 
        string[] depositorGroupCodes, 
        string[] signingDeviceCodes,
        CancellationToken cancellationToken)
    {
        var depositors = await sender.Send(new GetDepositorsByCodesQuery(depositorCodes), cancellationToken);
        if (depositors.Length != depositorCodes.Length)
        {
            logger.LogWarning("Sign - Settings - Did not find all depositors with codes: {DepositorCodes}.", string.Join(", ", depositorCodes));
            return UserErrors.ValidationDidNotFindAllDepositors;
        }
        
        var depositorGroups = await sender.Send(new GetDepositorGroupsByCodesQuery(depositorGroupCodes), cancellationToken);
        if (depositorGroups.Length != depositorGroupCodes.Length)
        {
            logger.LogWarning("Sign - Settings - Did not find all depositor groups with codes: {DepositorGroupCodes}.", string.Join(", ", depositorGroupCodes));
            return UserErrors.ValidationDidNotFindAllDepositorGroups;
        }
        
        var signingDevices = await sender.Send(new GetSigningDevicesByCodesQuery(signingDeviceCodes), cancellationToken);
        if (signingDevices.Length != signingDeviceCodes.Length)
        {
            logger.LogWarning("Sign - Settings - Did not find all signing devices with codes: {SigningDeviceCodes}.", string.Join(", ", signingDeviceCodes));
            return UserErrors.ValidationDidNotFindAllSigningDevices;
        }
        
        return new ValidationResult(
            depositors.Select(x => x.Code).ToArray(), 
            depositorGroups.Select(x => x.Code).ToArray(), 
            signingDevices.Select(x => x.Code).ToArray());
    }
    
    private record ValidationResult(
        string[] DepositorCodes, 
        string[] DepositorGroupCodes, 
        string[] SigningDeviceCodes);
}