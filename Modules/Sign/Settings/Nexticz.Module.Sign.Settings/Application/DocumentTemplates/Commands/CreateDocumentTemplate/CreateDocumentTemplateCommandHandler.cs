using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Sign.Settings.Application.DocumentTemplates.Queries.GetDocumentTemplateByCode;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.DocumentTemplateAggregate;
using Nexticz.Module.Sign.Settings.Domain.DocumentTemplateAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.DocumentTemplates.Commands.CreateDocumentTemplate;

internal class CreateDocumentTemplateCommandHandler(
        ILogger<CreateDocumentTemplateCommandHandler> logger,
        ISettingsUnitOfWork unitOfWork,
        ISender sender,
        IClock clock
    ) : IRequestHandler<CreateDocumentTemplateCommand, ErrorOr<DocumentTemplate>>
{
    public async Task<ErrorOr<DocumentTemplate>> Handle(CreateDocumentTemplateCommand request, CancellationToken cancellationToken)
    {
        var existingDocumentTemplate = await sender.Send(new GetDocumentTemplateByCodeQuery(request.Code), cancellationToken);

        if (existingDocumentTemplate.HasValue())
        {
            logger.LogInformation("Sign - Settings - Object {ObjectName} with code: {Code} already exists. Nothing to create.",
                nameof(DocumentTemplate), request.Code);
            return DocumentTemplateErrors.ValidationCodeAlreadyExists;
        }

        var textOffsets = request.TextOffsets.Select(x => new TextOffset(x.Name, x.Left, x.Bottom, x.Width, x.Height)).ToArray();
        var textBackgrounds = request.TextBackgrounds.Select(x => new TextBackground(x.Name, x.XPositionOffset, x.Width)).ToArray();
        var createdDocumentTemplate = DocumentTemplate.CreateNew(request.Code, textOffsets, textBackgrounds, clock.UtcNowOffset);

        if (createdDocumentTemplate.IsError)
        {
            logger.LogWarning("Sign - Settings - cannot create object {ObjectName} with code: {Code}. ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}.", 
                nameof(DocumentTemplate), request.Code, createdDocumentTemplate.FirstError.Code, createdDocumentTemplate.FirstError.Description);
            return createdDocumentTemplate.Errors;
        }
        
        var documentTemplatedCreatedEvent =
            new DocumentTemplateCreatedEvent(createdDocumentTemplate.Value.Id, createdDocumentTemplate.Value.Code, 
                createdDocumentTemplate.Value.TextOffsets, createdDocumentTemplate.Value.TextBackgrounds, 
                createdDocumentTemplate.Value.CreatedAt);

        unitOfWork.StartStream<DocumentTemplateCreatedEvent, DocumentTemplate>(createdDocumentTemplate.Value.Id, documentTemplatedCreatedEvent);
            
        logger.LogInformation("Sign - Settings - Object {ObjectName} with code: {Code} created, Id: {Id}",
            nameof(DocumentTemplate), createdDocumentTemplate.Value.Code, createdDocumentTemplate.Value.Id);
        
        return createdDocumentTemplate.Value;
    }
}