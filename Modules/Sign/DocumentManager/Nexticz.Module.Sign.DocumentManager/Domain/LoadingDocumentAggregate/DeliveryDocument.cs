using DocumentFormat.OpenXml.Drawing.Charts;
using ErrorOr;
using Nexticz.Module.Sign.SharedKernel.DomainCore;

namespace Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

public class DeliveryDocument : Entity
{
    public string Code { get; private set; }
    public string LoadingDocumentCode { get; private set; }
    public string PartnerCode { get; private set; }
    public string PartnerNameShort { get; private set; }
    public string DeliveryMethodCode { get; private set; }
    public string? DeliveryMethodName { get; private set; }
    public string WarehouseCode { get; private set; }
    public string PartnersOrderNumber { get; private set; }
    public string OperationalUnitCode { get; private set; }
    public string OperationalUnitName { get; private set; }
    public DateOnly IssueDate { get; private set; }
    public decimal? WeightCalculated { get; private set; }
    public int? AdrPoints { get; private set; }
    public string? RznoCode { get; private set; }
    public string? CombinedRznoCode { get; private set; } 
    public int PrintCopiesCount { get; private set; }
    public string? SigningDeviceCode { get; private set; }
    public DateTimeOffset? SentToSigningDeviceAt { get; private set; }
    public string? SentToSigningDeviceByUserName { get; private set; }
    public DateTimeOffset? FinishedAt { get; private set; }
    public string? FinishedByUserName { get; private set; }
    public FinishMethod? FinishMethodType { get; private set; }
    public string? FinishedByUserFullName { get; private set; }
    public string? DeletionReason { get; private set; }
    public string? SignedByDriverName { get; private set; }
    public string? SignedWithLicensePlate { get; private set; }
    public List<SentEmail> SentEmails { get; private set; }
    public bool IsFinished => FinishedAt is not null;
    
    // We need private constructor due to Marten deserialization
    private DeliveryDocument() {}
    
    public DeliveryDocument(
        string code,
        string loadingDocumentCode,
        string partnerCode,
        string partnerNameShort,
        string deliveryMethodCode,
        string? deliveryMethodName,
        string warehouseCode,
        string partnersOrderNumber,
        string operationalUnitCode,
        string operationalUnitName,
        DateOnly issueDate,       
        decimal? weightCalculated,
        int? adrPoints,
        string? rznoCode,
        string? combinedRznoCode,
        int printCopiesCount,
        string? signingDeviceCode = null,
        DateTimeOffset? sentToSigningDeviceAt = null,
        string? sentToSigningDeviceByUserName = null,
        DateTimeOffset? finishedAt = null,
        string? finishedByUserName = null,
        FinishMethod? finishMethodType = null,
        string? finishedByUserFullName = null,  
        string? deletionReason = null,
        string? signedByDriverName = null,
        string? signedWithLicensePlate = null,
        List<SentEmail>? sentEmails = null,       
        Guid? id = null
    ) : base(id ?? Guid.NewGuid())
    {
        Code = code;
        LoadingDocumentCode = loadingDocumentCode;
        PartnerCode = partnerCode;
        PartnerNameShort = partnerNameShort;
        DeliveryMethodCode = deliveryMethodCode;
        DeliveryMethodName = deliveryMethodName;
        WarehouseCode = warehouseCode;
        PartnersOrderNumber = partnersOrderNumber;
        OperationalUnitCode = operationalUnitCode;
        OperationalUnitName = operationalUnitName;
        WeightCalculated = weightCalculated;
        IssueDate = issueDate;       
        AdrPoints = adrPoints;
        RznoCode = rznoCode;
        CombinedRznoCode = combinedRznoCode;
        PrintCopiesCount = printCopiesCount;
        SigningDeviceCode = signingDeviceCode;
        SentToSigningDeviceAt = sentToSigningDeviceAt;
        SentToSigningDeviceByUserName = sentToSigningDeviceByUserName;
        FinishedAt = finishedAt;
        FinishedByUserName = finishedByUserName; 
        FinishMethodType = finishMethodType;
        FinishedByUserFullName = finishedByUserFullName;
        DeletionReason = deletionReason;
        SignedByDriverName = signedByDriverName;
        SignedWithLicensePlate = signedWithLicensePlate;
        SentEmails = sentEmails ?? [];
    }
    
    public void ReturnDeliveryDocument()
    {
        SigningDeviceCode = null;
        SentToSigningDeviceAt = null;
        SentToSigningDeviceByUserName = null;
    }
    
    public void SendDeliveryDocument(string signingDeviceCode, DateTimeOffset sentToSigningDeviceAt, string sentToSigningDeviceByUserName)
    {
        SigningDeviceCode = signingDeviceCode;
        SentToSigningDeviceAt = sentToSigningDeviceAt;
        SentToSigningDeviceByUserName = sentToSigningDeviceByUserName;
    }

    public void ManuallySignDocument(DateTimeOffset signedAt, string signedByUserName, string? signedByUserFullName)
    {
        FinishedAt = signedAt;
        FinishedByUserName = signedByUserName;
        FinishMethodType = FinishMethod.ManuallyUploaded;
        FinishedByUserFullName = signedByUserFullName;       
    }

    public void SignDocument(DateTimeOffset signedAt, string signedByUserFullName, string signedByUserName, string driverName, string licensePlate)
    {
        FinishedAt = signedAt;
        FinishedByUserName = signedByUserName;
        FinishMethodType = FinishMethod.Signed;
        FinishedByUserFullName = signedByUserFullName;
        SignedByDriverName = driverName;
        SignedWithLicensePlate = licensePlate;
    }
    
    public void DeleteDocument(DateTimeOffset deletedAt, string deletedByUserName, string? deletedByUserFullName, string? deletionReason)
    {
        FinishedAt = deletedAt;
        FinishedByUserName = deletedByUserName;
        FinishMethodType = FinishMethod.Deleted;       
        FinishedByUserFullName = deletedByUserFullName;       
        DeletionReason = deletionReason;
    }
    
    public void AddSentEmail(Guid emailId, string[] recipients, 
        DateTimeOffset completedAt, int attachmentsCount, string? failureReason)
    {
        SentEmails.Add(new SentEmail(
            emailId, recipients, completedAt, attachmentsCount, failureReason));
    }

    public ErrorOr<Success> ChangePrintCopiesCount(int printCopiesCount)
    {
        if (IsFinished)
            return LoadingDocumentDomainErrors.ValidationDeliveryDocumentIsAlreadyFinished(LoadingDocumentCode, Code);

        PrintCopiesCount = printCopiesCount;

        return Result.Success;
    }
}