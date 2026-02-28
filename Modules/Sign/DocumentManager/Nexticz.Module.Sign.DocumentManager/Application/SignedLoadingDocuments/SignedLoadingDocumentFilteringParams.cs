using Nexticz.Lib.Shared.DevExtreme;

namespace Nexticz.Module.Sign.DocumentManager.Application.SignedLoadingDocuments;

public class SignedLoadingDocumentFilteringParams : BaseFilteringParams
{
    public SignedLoadingDocumentFilteringParams() { }
    
    public SignedLoadingDocumentFilteringParams(
        string? partnersOrderNumber,
        string? rznoCode,
        string? combinedRznoCode,
        string? partnerName,
        string? receiverName,       
        string? fullTextFilter,       
        BaseFilteringParams baseFilteringParams)
        : base(baseFilteringParams)
    {
        PartnersOrderNumber = partnersOrderNumber;
        RznoCode = rznoCode;
        CombinedRznoCode = combinedRznoCode;
        PartnerName = partnerName;
        ReceiverName = receiverName;
        FullTextFilter = fullTextFilter;       
    }
    
    public string? PartnersOrderNumber { get; init; }
    public string? RznoCode { get; init; }
    public string? CombinedRznoCode { get; init; }
    public string? PartnerName { get; init; }
    public string? ReceiverName { get; init; }
    public string? FullTextFilter { get; init; }
}