using Nexticz.Module.Sign.SharedKernel.DomainCore;

namespace Nexticz.Module.Sign.DocumentLoader.Domain.LoadingDocumentAggregate;

public class LoadingDocument : AggregateRoot
{
    public string Code { get; private set; }
    public string[] DeliveryDocumentCodes { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private LoadingDocument() {}

    public LoadingDocument(
        string code,
        string[] deliveryDocumentCodes,
        Guid? id = null
    ) : base(id ?? Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentNullException(nameof(code));
        }
        
        Code = code;
        DeliveryDocumentCodes = deliveryDocumentCodes;
    }
}