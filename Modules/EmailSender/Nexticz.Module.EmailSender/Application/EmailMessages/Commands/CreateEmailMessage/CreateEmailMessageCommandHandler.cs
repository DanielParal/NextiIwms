using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.EmailSender.Application.Interfaces;
using Nexticz.Module.EmailSender.Domain.EmailMessageAggregate;
using Nexticz.Module.EmailSender.Domain.EmailMessageAggregate.Events;

namespace Nexticz.Module.EmailSender.Application.EmailMessages.Commands.CreateEmailMessage;

internal class CreateEmailMessageCommandHandler(
    ILogger<CreateEmailMessageCommandHandler> logger,
    IEmailSenderUnitOfWork unitOfWork,
    IClock clock) : IRequestHandler<CreateEmailMessageCommand, ErrorOr<EmailMessage>>
{
    public Task<ErrorOr<EmailMessage>> Handle(CreateEmailMessageCommand request, CancellationToken cancellationToken)
    {
        var initiator = new EmailInitiator(
            request.SendEmailMessage.Initiator.ModuleName,
            request.SendEmailMessage.Initiator.EmailType,
            request.SendEmailMessage.Initiator.ShouldSendConfirmationMessage,
            request.SendEmailMessage.Initiator.InitiatorProperties);
        
        var emailMessage = 
            EmailMessage.CreateNew(
                request.SendEmailMessage.ToRecipients, 
                request.SendEmailMessage.CcRecipients, 
                request.SendEmailMessage.BccRecipients, 
                request.SendEmailMessage.Subject, 
                request.SendEmailMessage.TextBody,
                request.SendEmailMessage.HtmlBody,
                clock.UtcNowOffset,
                initiator,
                request.SendEmailMessage.AttachmentsFolderGuid);

        if (emailMessage.IsError)
        {
            logger.LogWarning("EmailSender - cannot create email message. ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}.",
                emailMessage.FirstError.Code, emailMessage.FirstError.Description);
            return Task.FromResult<ErrorOr<EmailMessage>>(emailMessage.Errors);       
        }
        
        var emailCreatedEvent =
            new EmailMessageCreatedEvent(
                emailMessage.Value.Id, emailMessage.Value.ToRecipients, emailMessage.Value.CcRecipients, 
                emailMessage.Value.BccRecipients, emailMessage.Value.Subject, 
                emailMessage.Value.TextBody, emailMessage.Value.HtmlBody, emailMessage.Value.CreatedAt,
                emailMessage.Value.Initiator, emailMessage.Value.AttachmentsFolderGuid);
        
        unitOfWork.StartStream<EmailMessageCreatedEvent, EmailMessage>(emailMessage.Value.Id, emailCreatedEvent);
        
        logger.LogInformation("EmailSender - email with id: {Id} created.", emailMessage.Value.Id);
        
        return Task.FromResult<ErrorOr<EmailMessage>>(emailMessage.Value);
    }
}