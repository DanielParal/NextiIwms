using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.EmailSender.Application.Interfaces;
using Nexticz.Module.EmailSender.Domain.EmailMessageAggregate;
using Nexticz.Module.EmailSender.Domain.EmailMessageAggregate.Events;

namespace Nexticz.Module.EmailSender.Application.EmailMessages.Commands.ScheduleEmailMessage;

internal class ScheduleEmailMessageCommandHandler(
    ILogger<ScheduleEmailMessageCommandHandler> logger,
    IEmailSenderUnitOfWork unitOfWork,
    IClock clock) : IRequestHandler<ScheduleEmailMessageCommand, ErrorOr<EmailMessage>>
{
    public Task<ErrorOr<EmailMessage>> Handle(ScheduleEmailMessageCommand request, CancellationToken cancellationToken)
    {
        var initiator = new EmailInitiator(
            request.QueueEmailMessage.Initiator.ModuleName,
            request.QueueEmailMessage.Initiator.EmailType,
            request.QueueEmailMessage.Initiator.ShouldSendConfirmationMessage,
            request.QueueEmailMessage.Initiator.InitiatorProperties);
        
        var emailMessage = 
            EmailMessage.CreateNew(
                request.QueueEmailMessage.ToRecipients, 
                request.QueueEmailMessage.CcRecipients, 
                request.QueueEmailMessage.BccRecipients, 
                request.QueueEmailMessage.Subject, 
                request.QueueEmailMessage.TextBody,
                request.QueueEmailMessage.HtmlBody,
                clock.UtcNowOffset,
                initiator,
                request.QueueEmailMessage.AttachmentsFolderGuid);
        
        if (emailMessage.IsError)
        {
            logger.LogWarning("EmailSender - cannot schedule email message. ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}.",
                emailMessage.FirstError.Code, emailMessage.FirstError.Description);
            return Task.FromResult<ErrorOr<EmailMessage>>(emailMessage.Errors);       
        }
        
        emailMessage.Value.ScheduleEmail(request.QueueEmailMessage.ScheduledToBeSentAt);
        
        var emailMessageScheduledEvent =
            new EmailMessageScheduledEvent(
                emailMessage.Value.Id, emailMessage.Value.ToRecipients, emailMessage.Value.CcRecipients, 
                emailMessage.Value.BccRecipients, emailMessage.Value.Subject, 
                emailMessage.Value.TextBody, emailMessage.Value.HtmlBody, emailMessage.Value.CreatedAt,
                emailMessage.Value.ScheduledToBeSentAt!.Value, emailMessage.Value.Initiator, 
                emailMessage.Value.AttachmentsFolderGuid);
        
        unitOfWork.StartStream<EmailMessageScheduledEvent, EmailMessage>(emailMessage.Value.Id, emailMessageScheduledEvent);
        
        logger.LogInformation("EmailSender - email with id: {Id} scheduled. Schedule to be sent at: {ScheduledTime}", 
            emailMessage.Value.Id, emailMessage.Value.ScheduledToBeSentAt);
        
        return Task.FromResult<ErrorOr<EmailMessage>>(emailMessage.Value);
    }
}