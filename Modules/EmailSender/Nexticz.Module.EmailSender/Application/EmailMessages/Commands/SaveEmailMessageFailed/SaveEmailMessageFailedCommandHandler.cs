using MediatR;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.EmailSender.Application.EmailMessages.Queries.GetEmailMessageById;
using Nexticz.Module.EmailSender.Application.Interfaces;
using Nexticz.Module.EmailSender.Domain.EmailMessageAggregate;
using Nexticz.Module.EmailSender.Domain.EmailMessageAggregate.Events;

namespace Nexticz.Module.EmailSender.Application.EmailMessages.Commands.SaveEmailMessageFailed;

internal class SaveEmailMessageFailedCommandHandler(
    ILogger<SaveEmailMessageFailedCommandHandler> logger,
    ISender sender,
    IEmailSenderUnitOfWork unitOfWork,
    IClock clock) : IRequestHandler<SaveEmailMessageFailedCommand, ErrorOr<EmailMessage>>
{
    public async Task<ErrorOr<EmailMessage>> Handle(SaveEmailMessageFailedCommand request, CancellationToken cancellationToken)
    {
        var emailMessage = await sender.Send(new GetEmailMessageByIdQuery(request.Id), cancellationToken);

        if (emailMessage.IsError)
        {
            logger.LogWarning("EmailSender - cannot save failed email message with id: {Id}. Email message does not exist.",
                request.Id);
            return EmailMessageErrors.ValidationEmailMessageDoesNotExist;      
        }
        
        emailMessage.Value.FailEmail(request.ErrorMessage, clock); 
        
        var emailSentEvent =
            new EmailMessageFailedEvent(
                emailMessage.Value.Id, emailMessage.Value.SentAt!.Value, emailMessage.Value.ErrorMessage!);
        unitOfWork.AppendEvent(emailMessage.Value.Id, emailSentEvent);
        
        logger.LogInformation("EmailSender - email with id: {Id} failed to send at: {SentAt}. ErrorMessage: {ErrorMessage}.", 
            emailMessage.Value.Id, emailMessage.Value.SentAt, emailMessage.Value.ErrorMessage);
        
        return emailMessage.Value;
    }
}