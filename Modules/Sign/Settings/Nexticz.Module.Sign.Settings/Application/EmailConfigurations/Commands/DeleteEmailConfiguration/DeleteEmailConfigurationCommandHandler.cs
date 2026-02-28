using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Application.EmailConfigurations.Queries.GetEmailConfigurationById;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.EmailConfigurationAggregate;
using Nexticz.Module.Sign.Settings.Domain.EmailConfigurationAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.EmailConfigurations.Commands.DeleteEmailConfiguration;

internal class DeleteEmailConfigurationCommandHandler(
    ISender sender,
    ILogger<DeleteEmailConfigurationCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork) : IRequestHandler<DeleteEmailConfigurationCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(DeleteEmailConfigurationCommand request, CancellationToken cancellationToken)
    {
        var existingEmailConfiguration =
            await sender.Send(
                new GetEmailConfigurationByIdQuery(request.Id), 
                cancellationToken);

        if (existingEmailConfiguration.IsError)
        {
            logger.LogInformation("Sign - Settings - Object {ObjectName} with id: {Id} does not exist. Nothing to delete.",
                nameof(EmailConfiguration), request.Id);
            return EmailConfigurationErrors.ValidationIdDoesNotExist;
        }
        
        var deleteEvent = new EmailConfigurationDeletedV2Event(
            existingEmailConfiguration.Value.Id,
            existingEmailConfiguration.Value.DepositorCodes,
            existingEmailConfiguration.Value.PartnerCodes,
            existingEmailConfiguration.Value.ReceiverAndPartnerCombinationCodes);
        unitOfWork.AppendEvent(existingEmailConfiguration.Value.Id, deleteEvent);
        
        logger.LogInformation("Sign - Settings - Object {ObjectName} with id: {Id} and codes - " +
                              "DepositorCodes: {DepositorCodes}, PartnerCodes: {PartnerCodes}, ReceiverCodes: {ReceiverCodes} deleted.",
            nameof(EmailConfiguration), existingEmailConfiguration.Value.Id, existingEmailConfiguration.Value.DepositorCodes, 
            existingEmailConfiguration.Value.PartnerCodes, existingEmailConfiguration.Value.ReceiverAndPartnerCombinationCodes);
        return Result.Success;
    }
}