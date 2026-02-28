using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Emails;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Sign.Settings.Application.EmailConfigurations.Queries.GetEmailConfigurationByExactCodes;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.EmailConfigurationAggregate;
using Nexticz.Module.Sign.Settings.Domain.EmailConfigurationAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.EmailConfigurations.Commands.CreateEmailConfiguration;

internal class CreateEmailConfigurationCommandHandler(
    ISender sender,
    ILogger<CreateEmailConfigurationCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork) : IRequestHandler<CreateEmailConfigurationCommand, ErrorOr<EmailConfiguration>>
{
    public async Task<ErrorOr<EmailConfiguration>> Handle(CreateEmailConfigurationCommand request, CancellationToken cancellationToken)
    {
        var existingEmailConfiguration =
            await sender.Send(
                new GetEmailConfigurationByExactCodesQuery(
                    request.CreateRequest.DepositorCodes, request.CreateRequest.PartnerCodes, request.CreateRequest.ReceiverAndPartnerCombinationCodes), 
                cancellationToken);

        if (existingEmailConfiguration.HasValue())
        {
            logger.LogInformation("Sign - Settings - Object {ObjectName} with codes already exists. Nothing to create. " +
                                  "DepositorCodes: {DepositorCodes}, PartnerCodes: {PartnerCodes}, ReceiverCodes: {ReceiverCodes}",
                nameof(EmailConfiguration), request.CreateRequest.DepositorCodes, request.CreateRequest.PartnerCodes, request.CreateRequest.ReceiverAndPartnerCombinationCodes);
            return EmailConfigurationErrors.ValidationCombinationOfCodesAlreadyExists;
        }

        if (request.CreateRequest.RecipientEmailAddresses.Length == 0)
        {
            logger.LogInformation("Sign - Settings - Object {ObjectName}. No emails provided. Nothing to create. " +
                                  "DepositorCodes: {DepositorCodes}, PartnerCodes: {PartnerCodes}, ReceiverCodes: {ReceiverCodes}",
                nameof(EmailConfiguration), request.CreateRequest.DepositorCodes, request.CreateRequest.PartnerCodes, request.CreateRequest.ReceiverAndPartnerCombinationCodes);
            return EmailConfigurationErrors.ValidationNoEmailProvided;
        }
        
        foreach (var recipient in request.CreateRequest.RecipientEmailAddresses)
        {
            if (EmailValidator.IsValidEmail(recipient))
                continue;
            
            logger.LogInformation("Sign - Settings - Object {ObjectName}. Invalid email address: {InvalidRecipient}. Nothing to create. " +
                                  "DepositorCodes: {DepositorCodes}, PartnerCodes: {PartnerCodes}, ReceiverCodes: {ReceiverCodes}",
                nameof(EmailConfiguration), recipient, request.CreateRequest.DepositorCodes, request.CreateRequest.PartnerCodes, request.CreateRequest.ReceiverAndPartnerCombinationCodes);
            return EmailConfigurationErrors.ValidationInvalidEmail(recipient); 
        }

        var emailConfiguration = EmailConfiguration.CreateNew(
            request.CreateRequest.DepositorCodes,
            request.CreateRequest.PartnerCodes,
            request.CreateRequest.ReceiverAndPartnerCombinationCodes,
            request.CreateRequest.ShouldSendDeliveryDocument,
            request.CreateRequest.ShouldSendLoadingDocument,
            request.CreateRequest.RecipientEmailAddresses,
            request.CreateRequest.ShouldSendImmediately);
        
        if (emailConfiguration.IsError)
        {
            logger.LogInformation("Sign - Settings - Object {ObjectName} cannot be created because of domain validation. " +
                                  "ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}, " +
                                  "DepositorCodes: {DepositorCodes}, PartnerCodes: {PartnerCodes}, ReceiverCodes: {ReceiverCodes}",
                nameof(EmailConfiguration), emailConfiguration.FirstError.Code, emailConfiguration.FirstError.Description,
                request.CreateRequest.DepositorCodes, request.CreateRequest.PartnerCodes, request.CreateRequest.ReceiverAndPartnerCombinationCodes);
            return emailConfiguration.Errors;
        }
        
        var emailConfigurationValue = emailConfiguration.Value;
        var emailConfigurationCreatedEvent = new EmailConfigurationCreatedV2Event(
            emailConfigurationValue.Id, emailConfigurationValue.DepositorCodes, emailConfigurationValue.PartnerCodes, 
            emailConfigurationValue.ReceiverAndPartnerCombinationCodes, emailConfigurationValue.ShouldSendDeliveryDocument, emailConfigurationValue.ShouldSendLoadingDocument,
            emailConfigurationValue.RecipientEmailAddresses, emailConfigurationValue.ShouldSendImmediately, emailConfigurationValue.Type);
        unitOfWork.StartStream<EmailConfigurationCreatedV2Event, EmailConfiguration>(emailConfigurationValue.Id, emailConfigurationCreatedEvent);
        
        logger.LogInformation("Sign - Settings - Object {ObjectName} with codes - " +
                              "DepositorCodes: {DepositorCodes}, PartnerCodes: {PartnerCodes}, ReceiverCodes: {ReceiverCodes} created",
            nameof(EmailConfiguration), request.CreateRequest.DepositorCodes, request.CreateRequest.PartnerCodes, request.CreateRequest.ReceiverAndPartnerCombinationCodes);
        return emailConfiguration;
    }
}