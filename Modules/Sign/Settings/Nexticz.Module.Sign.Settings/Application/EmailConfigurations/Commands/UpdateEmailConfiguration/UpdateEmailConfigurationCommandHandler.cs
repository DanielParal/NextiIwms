using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Emails;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Sign.Settings.Application.EmailConfigurations.Queries.GetEmailConfigurationByExactCodes;
using Nexticz.Module.Sign.Settings.Application.EmailConfigurations.Queries.GetEmailConfigurationById;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.EmailConfigurationAggregate;
using Nexticz.Module.Sign.Settings.Domain.EmailConfigurationAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.EmailConfigurations.Commands.UpdateEmailConfiguration;

internal class UpdateEmailConfigurationCommandHandler(
    ISender sender,
    ILogger<UpdateEmailConfigurationCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork) : IRequestHandler<UpdateEmailConfigurationCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(UpdateEmailConfigurationCommand request, CancellationToken cancellationToken)
    {
        var existingEmailConfigurationWitId =
            await sender.Send(
                new GetEmailConfigurationByIdQuery(request.Id), 
                cancellationToken);

        if (existingEmailConfigurationWitId.IsError)
        {
            logger.LogInformation("Sign - Settings - Object {ObjectName} with id: {Id} does not exist. Nothing to update.",
                nameof(EmailConfiguration), request.Id);
            return EmailConfigurationErrors.ValidationIdDoesNotExist;
        }
        
        var existingEmailConfigurationCodesCombination =
            await sender.Send(
                new GetEmailConfigurationByExactCodesQuery(
                    request.UpdateRequest.DepositorCodes, request.UpdateRequest.PartnerCodes, request.UpdateRequest.ReceiverAndPartnerCombinationCodes), 
                cancellationToken);

        if (existingEmailConfigurationCodesCombination.HasValue() && existingEmailConfigurationWitId.Value.Id != existingEmailConfigurationCodesCombination.Value.Id)
        {
            logger.LogInformation("Sign - Settings - Object {ObjectName} with codes already exists. Nothing to update. " +
                                  "DepositorCodes: {DepositorCodes}, PartnerCodes: {PartnerCodes}, ReceiverCodes: {ReceiverCodes}",
                nameof(EmailConfiguration), request.UpdateRequest.DepositorCodes, request.UpdateRequest.PartnerCodes, request.UpdateRequest.ReceiverAndPartnerCombinationCodes);
            return EmailConfigurationErrors.ValidationCombinationOfCodesAlreadyExists;
        }

        if (request.UpdateRequest.RecipientEmailAddresses.Length == 0)
        {
            logger.LogInformation("Sign - Settings - Object {ObjectName}. No emails provided. Nothing to update. " +
                                  "DepositorCodes: {DepositorCodes}, PartnerCodes: {PartnerCodes}, ReceiverCodes: {ReceiverCodes}",
                nameof(EmailConfiguration), request.UpdateRequest.DepositorCodes, request.UpdateRequest.PartnerCodes, request.UpdateRequest.ReceiverAndPartnerCombinationCodes);
            return EmailConfigurationErrors.ValidationNoEmailProvided;
        }
        
        foreach (var recipient in request.UpdateRequest.RecipientEmailAddresses)
        {
            if (EmailValidator.IsValidEmail(recipient))
                continue;
            
            logger.LogInformation("Sign - Settings - Object {ObjectName}. Invalid email address: {InvalidRecipient}. Nothing to update. " +
                                  "DepositorCodes: {DepositorCodes}, PartnerCodes: {PartnerCodes}, ReceiverCodes: {ReceiverCodes}",
                nameof(EmailConfiguration), recipient, request.UpdateRequest.DepositorCodes, request.UpdateRequest.PartnerCodes, request.UpdateRequest.ReceiverAndPartnerCombinationCodes);
            return EmailConfigurationErrors.ValidationInvalidEmail(recipient); 
        }

        var currentEmailConfiguration = existingEmailConfigurationWitId.Value;
        var emailConfiguration = currentEmailConfiguration.Update(
            request.UpdateRequest.DepositorCodes,
            request.UpdateRequest.PartnerCodes,
            request.UpdateRequest.ReceiverAndPartnerCombinationCodes,
            request.UpdateRequest.ShouldSendDeliveryDocument,
            request.UpdateRequest.ShouldSendLoadingDocument,
            request.UpdateRequest.RecipientEmailAddresses,
            request.UpdateRequest.ShouldSendImmediately);
        
        if (emailConfiguration.IsError)
        {
            logger.LogInformation("Sign - Settings - Object {ObjectName} cannot be updated because of domain validation. " +
                                  "ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}, " +
                                  "DepositorCodes: {DepositorCodes}, PartnerCodes: {PartnerCodes}, ReceiverCodes: {ReceiverCodes}",
                nameof(EmailConfiguration), emailConfiguration.FirstError.Code, emailConfiguration.FirstError.Description,
                request.UpdateRequest.DepositorCodes, request.UpdateRequest.PartnerCodes, request.UpdateRequest.ReceiverAndPartnerCombinationCodes);
            return emailConfiguration.Errors;
        }
        
        var emailConfigurationUpdatedEvent = new EmailConfigurationUpdatedV2Event(
            currentEmailConfiguration.Id, currentEmailConfiguration.DepositorCodes, currentEmailConfiguration.PartnerCodes, 
            currentEmailConfiguration.ReceiverAndPartnerCombinationCodes, currentEmailConfiguration.ShouldSendDeliveryDocument, currentEmailConfiguration.ShouldSendLoadingDocument,
            currentEmailConfiguration.RecipientEmailAddresses, currentEmailConfiguration.ShouldSendImmediately, currentEmailConfiguration.Type);
        unitOfWork.AppendEvent(currentEmailConfiguration.Id, emailConfigurationUpdatedEvent);
        
        logger.LogInformation("Sign - Settings - Object {ObjectName} with id: {Id} and with codes - " +
                              "DepositorCodes: {DepositorCodes}, PartnerCodes: {PartnerCodes}, ReceiverCodes: {ReceiverCodes} updated.",
            nameof(EmailConfiguration), request.Id, request.UpdateRequest.DepositorCodes, request.UpdateRequest.PartnerCodes, request.UpdateRequest.ReceiverAndPartnerCombinationCodes);
        
        return Result.Success;
    }
}