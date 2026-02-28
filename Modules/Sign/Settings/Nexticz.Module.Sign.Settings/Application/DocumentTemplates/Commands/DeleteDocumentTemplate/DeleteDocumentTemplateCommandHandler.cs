using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Sign.Settings.Application.Depositors.Queries.GetDepositorsByTemplateCode;
using Nexticz.Module.Sign.Settings.Application.DocumentTemplates.Queries.GetDocumentTemplateByCode;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.DocumentTemplateAggregate;
using Nexticz.Module.Sign.Settings.Domain.DocumentTemplateAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.DocumentTemplates.Commands.DeleteDocumentTemplate;

internal class DeleteDocumentTemplateCommandHandler(
    ILogger<DeleteDocumentTemplateCommandHandler> logger,
    ISender sender,
    ISettingsUnitOfWork unitOfWork,
    IClock clock
) 
    : IRequestHandler<DeleteDocumentTemplateCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteDocumentTemplateCommand request, CancellationToken cancellationToken)
    {
        var documentTemplate = await sender.Send(new GetDocumentTemplateByCodeQuery(request.Code), cancellationToken);

        if (documentTemplate.IsError)
        {
            logger.LogInformation("Sign - Settings - Did not find object {ObjectName} with code: {Code}. Nothing to delete", nameof(DocumentTemplate), request.Code);
            return DocumentTemplateErrors.ValidationCodeDoesNotExist;
        }
        
        var existingDepositors = await sender.Send(new GetDepositorsByTemplateCodeQuery(request.Code), cancellationToken);
        if (existingDepositors.Length > 0)
        {
            logger.LogWarning("Sign - Settings - cannot delete object {ObjectName} with code: {Code} because it is still used in depositors, DepositorCodes: {DepositorCodes}. Nothing to delete", 
                nameof(DocumentTemplate), request.Code, existingDepositors.Select(x => x.Code));
            return DocumentTemplateErrors.ValidationDocumentTemplateIsUsedInDepositors;
        }
        
        var documentTemplateDeletedEvent = new DocumentTemplateDeletedEvent(documentTemplate.Value.Id, clock.UtcNowOffset);
        unitOfWork.AppendEvent(documentTemplate.Value.Id, documentTemplateDeletedEvent);
        
        logger.LogInformation("Sign - Settings - Object {ObjectName} with code: {Code} deleted",
            nameof(DocumentTemplate), request.Code);
        return Result.Deleted;
    }
}