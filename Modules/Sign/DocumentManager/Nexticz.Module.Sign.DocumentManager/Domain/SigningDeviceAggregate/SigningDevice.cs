using Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate.Events;
using Nexticz.Module.Sign.SharedKernel.DomainCore;

namespace Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate;

public class SigningDevice : AggregateRoot
{
    public string Name { get; private set; }
    public string Code { get; private set; }
    public bool IsActive { get; private set; }
    public string PrinterCode { get; private set; }
    public string? SentLoadingDocumentsByUserName { get; private set; }
    public DateTimeOffset? SentLoadingDocumentsAt { get; private set; }
    public SentLoadingDocument[] SentDocuments { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private SigningDevice() {}

    public SigningDevice(
        string code,
        string name,
        bool isActive,
        string printerCode,
        SentLoadingDocument[] sentDocuments,
        string? sentLoadingDocumentsByUserName = null,
        DateTimeOffset? sentLoadingDocumentsAt = null,       
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Code = code.ToUpperInvariant();
        Name = name;
        IsActive = isActive;
        PrinterCode = printerCode;
        SentDocuments = sentDocuments;
        SentLoadingDocumentsByUserName = sentLoadingDocumentsByUserName;
        SentLoadingDocumentsAt = sentLoadingDocumentsAt;   
    }
    
    public bool HasSentDocuments() => SentDocuments.Length > 0;

    public void Apply(SigningDeviceCreatedEvent @event)
    {
        Code = @event.Code;
        Name = @event.Name;
        IsActive = @event.IsActive;
        PrinterCode = @event.PrinterCode;
        SentDocuments = [];
        SentLoadingDocumentsByUserName = null;
        SentLoadingDocumentsAt = null;   
    }
    
    public void Apply(SigningDeviceUpdatedEvent @event)
    {
        Name = @event.Name;
        IsActive = @event.IsActive;   
        PrinterCode = @event.PrinterCode;
    }
    
    public void Apply(DocumentsFromSigningDeviceReturnedEvent @event)
    {
        SentDocuments = [];
        SentLoadingDocumentsByUserName = null;
        SentLoadingDocumentsAt = null;  
    }
    
    public void Apply(DocumentsToSigningDeviceSentEvent @event)
    {
        SentDocuments = @event.SentLoadingDocuments;
        SentLoadingDocumentsByUserName = @event.SentByUserName;
        SentLoadingDocumentsAt = @event.SentAt; 
    }
    
    public void Apply(SigningResultSavedEvent @event)
    {
        if (@event.SigningResult.IsFailure) 
            return;
        
        SentDocuments = [];
        SentLoadingDocumentsByUserName = null;
        SentLoadingDocumentsAt = null;
    }
}