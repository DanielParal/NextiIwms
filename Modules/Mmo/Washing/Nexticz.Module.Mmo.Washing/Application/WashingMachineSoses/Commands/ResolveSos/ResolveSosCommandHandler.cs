using System.Text.Json;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Contracts.Users;
using Nexticz.Module.Mmo.Settings.Contracts.Users.Queries;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.Washing.Contracts.Batches;
using Nexticz.Module.Notifications.Contracts.SignalRNotifications;

using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Application.MessagePublishers;
using Nexticz.Module.Mmo.Washing.Application.WashingMachineSoses.Queries.GetWashingMachineSosByCode;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSosAggregate.Events;

namespace Nexticz.Module.Mmo.Washing.Application.WashingMachineSoses.Commands.ResolveSos;

internal class ResolveSosCommandHandler(
    ISender sender,
    ILogger<ResolveSosCommandHandler> logger,
    IClock clock,
    IWashingUnitOfWork unitOfWork,
    IMessagePublisher messagePublisher) : IRequestHandler<ResolveSosCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(ResolveSosCommand request, CancellationToken cancellationToken)
    {
        var washingMachineSos = await sender.Send(new GetWashingMachineSosByCodeQuery(request.WashingMachineCode), cancellationToken);

        if (washingMachineSos is null || !washingMachineSos.IsHelpNeeded)
        {
            logger.LogInformation("Washing - washing machine does not need resolve SOS. Washing machine code: {WashingMachineCode}.", 
                washingMachineSos?.Code);
            return Result.Success;
        }
        
        var sosResolvedEvent = new WashingMachineSosResolvedEvent(washingMachineSos.Id, washingMachineSos.Code, clock.UtcNowOffset);
        unitOfWork.AppendEvent(washingMachineSos.Id, sosResolvedEvent);
        
        logger.LogInformation("Washing - sos for washing machine: {WashingMachineCode} was resolved.", 
            request.WashingMachineCode);;

        await PublishNotificationAsync(washingMachineSos.Code, cancellationToken);
        
        return Result.Success;
    }
    
    private async Task PublishNotificationAsync(string washingMachineCode, CancellationToken cancellationToken)
    {
        var usersForNotification = await sender.Send(new GetUsersByNotificationQuery(ReceivableNotificationContract.SosCalled), cancellationToken);
        if (usersForNotification.Length == 0)
        {
            logger.LogWarning("Washing - there are no users for notification {NotificationName}. WashingMachineCode: {WashingMachineCode}.", 
                nameof(ReceivableNotificationContract.SosResolved), washingMachineCode);
            return;
        }
        
        var sosSignalRMessage = new SosSignalRMessage(washingMachineCode);
        var signalRPublishedNotification = 
            new SignalRPublishedNotification(
                WashingMachineSosTranslations.SosResolvedSignalRMessageTitle().TranslationValue,
                WashingMachineSosTranslations.SosResolvedSignalRMessageDescription(washingMachineCode).TranslationValue,
                JsonSerializer.Serialize(sosSignalRMessage), 
                nameof(ReceivableNotificationContract.SosResolved),
                ModuleNameProvider.Name, 
                NotificationSeverityContract.Success,
                usersForNotification.Select(x => x.UserName).ToArray());
        
        await messagePublisher.PublishAsync(signalRPublishedNotification, cancellationToken);
    }
}