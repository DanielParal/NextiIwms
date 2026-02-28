using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Sign.Settings.Application.EmailTemplates.Queries.GetEmailTemplateByCode;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.EmailTemplateAggregate;
using Nexticz.Module.Sign.Settings.Domain.EmailTemplateAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.EmailTemplates.Commands.UpdateEmailTemplate;

internal class UpdateEmailTemplateCommandHandler(
    ILogger<UpdateEmailTemplateCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender) : IRequestHandler<UpdateEmailTemplateCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(UpdateEmailTemplateCommand request, CancellationToken cancellationToken)
    {
        var emailTemplate = await sender.Send(new GetEmailTemplateByCodeQuery(request.Code), cancellationToken);
        if (!emailTemplate.HasValue())
        {
            logger.LogInformation("Sign - Settings - did not find object {ObjectName} with code: {Code}. Nothing to update.", 
                nameof(EmailTemplate), request.Code);
            return EmailTemplateErrors.ValidationEmailTemplateDoesNotExist;
        }
        
        var validationResult = emailTemplate.Value.Update( request.Name, request.Subject, request.HtmlBody, request.TextBody);
        
        if (validationResult.IsError)
        {
            logger.LogInformation("Sign - Settings - cannot update object {ObjectName} with code: {Code}. ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}.", 
                nameof(EmailTemplate), request.Code, validationResult.FirstError.Code, validationResult.FirstError.Description);
            return validationResult;
        }
        
        var emailTemplateUpdatedEvent = new EmailTemplateUpdatedEvent(
            emailTemplate.Value.Id, emailTemplate.Value.Code, request.Name, request.Subject, request.HtmlBody, request.TextBody);
        unitOfWork.AppendEvent(emailTemplate.Value.Id, emailTemplateUpdatedEvent);
        
        logger.LogInformation("Sign - Object {ObjectName} with code: {Code} updated.",
            nameof(EmailTemplate), request.Code);
        return Result.Success;
    }
}