using ErrorOr;
using Nexticz.Module.Sign.Settings.Domain.DocumentTemplateAggregate.Events;
using Nexticz.Module.Sign.SharedKernel.DomainCore;

namespace Nexticz.Module.Sign.Settings.Domain.DocumentTemplateAggregate;

public class DocumentTemplate : AggregateRoot
{
    public string Code { get; private set; }
    public TextOffset[] TextOffsets { get; private set; }
    public TextBackground[] TextBackgrounds { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private DocumentTemplate() {}

    private DocumentTemplate(
        string code,
        TextOffset[] textOffsets,
        TextBackground[] textBackgrounds,
        DateTimeOffset createdAt,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Code = code.ToUpperInvariant();
        TextOffsets = textOffsets;
        TextBackgrounds = textBackgrounds;
        CreatedAt = createdAt;
    }

    public static ErrorOr<DocumentTemplate> CreateNew(string code, 
        TextOffset[] textOffsets, TextBackground[] textBackgrounds, DateTimeOffset createdAt)
    {
        if (string.IsNullOrWhiteSpace(code))
            return DocumentTemplateDomainErrors.ValidationCodeIsRequired;
        
        var isValidTextOffsets = IsValidTextOffsets(textOffsets);
        if (isValidTextOffsets.IsError)
            return isValidTextOffsets.Errors;
        
        var isValidTextBackgrounds = IsValidTextBackgrounds(textBackgrounds);
        if (isValidTextBackgrounds.IsError)
            return isValidTextBackgrounds.Errors;
        
        return new DocumentTemplate(code, textOffsets, textBackgrounds, createdAt);
    }
    
    public ErrorOr<Success> Update(TextOffset[] textOffsets, TextBackground[] textBackgrounds)
    {
        var isValidTextOffsets = IsValidTextOffsets(textOffsets);
        if (isValidTextOffsets.IsError)
            return isValidTextOffsets.Errors;
        
        var isValidTextBackgrounds = IsValidTextBackgrounds(textBackgrounds);
        if (isValidTextBackgrounds.IsError)
            return isValidTextBackgrounds.Errors;
        
        TextOffsets = textOffsets;
        TextBackgrounds = textBackgrounds;
        
        return Result.Success;
    }

    private static ErrorOr<Success> IsValidTextOffsets(TextOffset[] textOffsets)
    {
        if (textOffsets.Any(x => string.IsNullOrWhiteSpace(x.Name)))
            return DocumentTemplateDomainErrors.ValidationEmptyTextOffsetNames;
        
        if (textOffsets.GroupBy(x => x.Name).Any(g => g.Count() > 1))
            return DocumentTemplateDomainErrors.ValidationDuplicateTextOffsetNames;
        
        return Result.Success;
    }
    
    private static ErrorOr<Success> IsValidTextBackgrounds(TextBackground[] textBackgrounds)
    {
        if (textBackgrounds.Any(x => string.IsNullOrWhiteSpace(x.Name)))
            return DocumentTemplateDomainErrors.ValidationEmptyTextBackgroundNames;
        
        if (textBackgrounds.GroupBy(x => x.Name).Any(g => g.Count() > 1))
            return DocumentTemplateDomainErrors.ValidationDuplicateTextBackgroundNames;
        
        return Result.Success;
    }

    public void Apply(DocumentTemplateCreatedEvent @event)
    {
        Code = @event.Code;
        TextOffsets = @event.TextOffsets;
        TextBackgrounds = @event.TextBackgrounds;
        CreatedAt = @event.CreatedAt;
    }
    
    public void Apply(DocumentTemplateUpdatedEvent @event)
    {
        TextOffsets = @event.TextOffsets;
        TextBackgrounds = @event.TextBackgrounds;
    }
}