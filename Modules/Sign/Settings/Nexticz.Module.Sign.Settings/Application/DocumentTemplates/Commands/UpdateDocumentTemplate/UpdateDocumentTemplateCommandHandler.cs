using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Sign.Settings.Application.DocumentTemplates.Queries.GetDocumentTemplateByCode;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.DocumentTemplateAggregate;
using Nexticz.Module.Sign.Settings.Domain.DocumentTemplateAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.DocumentTemplates.Commands.UpdateDocumentTemplate;

internal class UpdateDocumentTemplateCommandHandler(
    ILogger<UpdateDocumentTemplateCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender,
    IClock clock) 
    : IRequestHandler<UpdateDocumentTemplateCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateDocumentTemplateCommand request, CancellationToken cancellationToken)
    {
        var documentTemplate = await sender.Send(new GetDocumentTemplateByCodeQuery(request.Code), cancellationToken);

        if (documentTemplate.IsError)
        {
            logger.LogWarning("Sign - Settings - Did not find object {ObjectName} with code: {Code}. Nothing to update", 
                nameof(DocumentTemplate), request.Code);
            return DocumentTemplateErrors.ValidationCodeDoesNotExist;
        }
        
        var textOffsets = request.TextOffsetContracts.Select(x => new TextOffset(x.Name, x.Left, x.Bottom, x.Width, x.Height)).ToArray();
        var textBackgrounds = request.TextBackgroundContracts.Select(x => new TextBackground(x.Name, x.XPositionOffset, x.Width)).ToArray();

        var validationResult = documentTemplate.Value.Update(textOffsets, textBackgrounds);  
        
        if (validationResult.IsError)
        {
            logger.LogWarning("Sign - Settings - cannot update object {ObjectName} with code: {Code}. ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}.", 
                nameof(DocumentTemplate), request.Code, validationResult.FirstError.Code, validationResult.FirstError.Description);
            return validationResult.Errors;
        }
        
        var documentTemplateUpdatedEvent = new DocumentTemplateUpdatedEvent(
            documentTemplate.Value.Id, documentTemplate.Value.TextOffsets, documentTemplate.Value.TextBackgrounds, clock.UtcNowOffset);
        unitOfWork.AppendEvent(documentTemplate.Value.Id, documentTemplateUpdatedEvent);
        
        logger.LogInformation("Sign - Settings - Object {ObjectName} with code: {Code}, id: {Id} updated",
            nameof(DocumentTemplate), documentTemplate.Value.Code, documentTemplate.Value.Id);
        
        return Result.Updated;
    }
}