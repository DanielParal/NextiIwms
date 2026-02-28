using MediatR;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.EmailSender.Application.EmailMessages.Queries.GetEmailMessageById;
using Nexticz.Module.EmailSender.Application.Interfaces;
using Nexticz.Module.EmailSender.Domain.EmailMessageAggregate;
using Nexticz.Module.EmailSender.Domain.EmailMessageAggregate.Events;

namespace Nexticz.Module.EmailSender.Application.EmailMessages.Commands.SaveEmailMessageSent;

internal class SaveEmailMessageSentCommandHandler(
    ILogger<SaveEmailMessageSentCommandHandler> logger,
    ISender sender,
    IEmailSenderUnitOfWork unitOfWork,
    IClock clock) : IRequestHandler<SaveEmailMessageSentCommand, ErrorOr<EmailMessage>>
{
    public async Task<ErrorOr<EmailMessage>> Handle(SaveEmailMessageSentCommand request, CancellationToken cancellationToken)
    {
        var emailMessage = await sender.Send(new GetEmailMessageByIdQuery(request.Id), cancellationToken);

        if (emailMessage.IsError)
        {
            logger.LogWarning("EmailSender - cannot save sent email message with id: {Id}. Email message does not exist.",
                request.Id);
            return EmailMessageErrors.ValidationEmailMessageDoesNotExist;      
        }
        
        emailMessage.Value.SendEmail(clock, request.Attachments); 
        
        var emailSentEvent =
            new EmailMessageSentEvent(
                emailMessage.Value.Id, emailMessage.Value.SentAt!.Value, emailMessage.Value.Attachments);
        unitOfWork.AppendEvent(emailMessage.Value.Id, emailSentEvent);
        
        logger.LogInformation("EmailSender - email with id: {Id} sent at: {SentAt} with attachments count: {Attachments}.", 
            emailMessage.Value.Id, emailMessage.Value.SentAt, emailMessage.Value.Attachments?.Length ?? 0);
        
        return emailMessage.Value;
    }
}