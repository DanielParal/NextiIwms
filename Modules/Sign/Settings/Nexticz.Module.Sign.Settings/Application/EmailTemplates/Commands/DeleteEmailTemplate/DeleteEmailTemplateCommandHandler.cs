using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Sign.Settings.Application.EmailTemplates.Queries.GetEmailTemplateByCode;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.EmailTemplateAggregate;
using Nexticz.Module.Sign.Settings.Domain.EmailTemplateAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.EmailTemplates.Commands.DeleteEmailTemplate;

internal class DeleteEmailTemplateCommandHandler(
    ILogger<DeleteEmailTemplateCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender) : IRequestHandler<DeleteEmailTemplateCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(DeleteEmailTemplateCommand request, CancellationToken cancellationToken)
    {
        var emailTemplate = await sender.Send(new GetEmailTemplateByCodeQuery(request.Code), cancellationToken);
        if (!emailTemplate.HasValue())
        {
            logger.LogInformation("Sign - Settings - did not find object {ObjectName} with code: {Code}. Nothing to delete.", 
                nameof(EmailTemplate), request.Code);
            return EmailTemplateErrors.ValidationEmailTemplateDoesNotExist;
        }
        
        var emailTemplateDeletedEvent = new EmailTemplateDeletedEvent(emailTemplate.Value.Id, emailTemplate.Value.Code);
        unitOfWork.AppendEvent(emailTemplate.Value.Id, emailTemplateDeletedEvent);
        
        logger.LogInformation("Sign - Object {ObjectName} with code: {Code} deleted.",
            nameof(emailTemplate), request.Code);
        return Result.Success;
    }
}