using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Sign.Settings.Application.EmailTemplates.Queries.GetEmailTemplateByCode;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.EmailTemplateAggregate;
using Nexticz.Module.Sign.Settings.Domain.EmailTemplateAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.EmailTemplates.Commands.CreateEmailTemplate;

internal class CreateEmailTemplateCommandHandler(
    ILogger<CreateEmailTemplateCommandHandler> logger,
    ISender sender,
    ISettingsUnitOfWork unitOfWork) : IRequestHandler<CreateEmailTemplateCommand, ErrorOr<EmailTemplate>>
{
    public async Task<ErrorOr<EmailTemplate>> Handle(CreateEmailTemplateCommand request, CancellationToken cancellationToken)
    {
        var existingTemplate = await sender.Send(new GetEmailTemplateByCodeQuery(request.Code), cancellationToken);
        if (existingTemplate.HasValue())
        {
            logger.LogInformation("Sign - Settings - object {ObjectName} with code: {Code} already exists. Nothing to create.", 
                nameof(EmailTemplate), request.Code);
            return EmailTemplateErrors.ValidationEmailTemplateWithCodeAlreadyExists;
        }
        
        var emailTemplate = EmailTemplate.CreateNew(request.Code, request.Name, request.Subject, request.HtmlBody, request.TextBody);
        if (emailTemplate.IsError)
        {
            logger.LogInformation("Sign - Settings - cannot create object {ObjectName} with code: {Code}. ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}.", 
                nameof(EmailTemplate), request.Code, emailTemplate.FirstError.Code, emailTemplate.FirstError.Description);
            return emailTemplate.Errors;
        }

        var emailTemplateCreatedEvent = 
            new EmailTemplateCreatedEvent(
                emailTemplate.Value.Id,
                emailTemplate.Value.Code,
                emailTemplate.Value.Name,
                emailTemplate.Value.Subject,
                emailTemplate.Value.HtmlBody,
                emailTemplate.Value.TextBody);
        
        unitOfWork.StartStream<EmailTemplateCreatedEvent, EmailTemplate>(emailTemplate.Value.Id, emailTemplateCreatedEvent);
            
        logger.LogInformation("Sign - Object {ObjectName} with code: {Code} created.",
            nameof(EmailTemplate), request.Code);
        return emailTemplate.Value;
    }
}