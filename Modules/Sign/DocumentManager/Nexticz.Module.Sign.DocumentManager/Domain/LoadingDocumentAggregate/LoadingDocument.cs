using System.Text;
using ErrorOr;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate.Events;
using Nexticz.Module.Sign.SharedKernel.DomainCore;

namespace Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

public class LoadingDocument : AggregateRoot
{
    public string Code { get; private set; }
    public int? GateNumber { get; private set; }
    public string DepositorCode { get; private set; }
    public string? DepositorName { get; private set; }
    public string DeliveryMethodCode { get; private set; }
    public string? DeliveryMethodName { get; private set; }
    /// <summary>
    /// License plate which comes from the WMS system
    /// </summary>
    public string? OriginalLicensePlate { get; private set; }
    /// <summary>
    /// Driver name which comes from the WMS system
    /// </summary>
    public string? OriginalDriverName { get; private set; }
    public decimal? Weight { get; private set; }
    public int? AdrPoints { get; private set; }
    public string? LoadingLocation { get; private set; }
    public string LoadingInWmsFinishedBy { get; private set; }
    public DateTimeOffset LoadingInWmsFinishedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public int PrintCopiesCount { get; private set; }
    public string? SigningDeviceCode { get; private set; }
    public DateTimeOffset? SentToSigningDeviceAt { get; private set; }
    public string? SentToSigningDeviceByUserName { get; private set; }
    public DateTimeOffset? FinishedAt { get; private set; }
    public string? FinishedByUserName { get; private set; }
    public FinishMethod? FinishMethodType { get; private set; }
    public string? FinishedByUserFullName { get; private set; }
    public string? DeletionReason { get; private set; }
    /// <summary>
    /// Actual name of the driver which comes from the sign app
    /// </summary>
    public string? SignedByDriverName { get; private set; }
    /// <summary>
    /// Actual license plate of the driver which comes from the sign app
    /// </summary>
    public string? SignedWithLicensePlate { get; private set; }
    public DeliveryDocument[] DeliveryDocuments { get; private set; } = [];
    public List<SentEmail> SentEmails { get; private set; } = [];
    public bool IsFinished => FinishedAt is not null;
    
    // We need private constructor due to Marten deserialization
    private LoadingDocument() {}
    
    public LoadingDocument(
        string code,
        int? gateNumber,
        string depositorCode,
        string? depositorName,
        string deliveryMethodCode,
        string? deliveryMethodName,
        string? originalLicensePlate,
        string? originalDriverName,
        decimal? weight,
        int? adrPoints,
        string? loadingLocation,
        string loadingInWmsFinishedBy,
        DateTimeOffset loadingInWmsFinishedAt,
        DateTimeOffset createdAt,
        int printCopiesCount,
        DeliveryDocument[] deliveryDocuments,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Code = code;
        GateNumber = gateNumber;
        DepositorCode = depositorCode;
        DepositorName = depositorName;
        DeliveryMethodCode = deliveryMethodCode;
        DeliveryMethodName = deliveryMethodName;
        OriginalLicensePlate = originalLicensePlate;
        OriginalDriverName = originalDriverName;
        Weight = weight;
        AdrPoints = adrPoints;
        LoadingLocation = loadingLocation;
        LoadingInWmsFinishedBy = loadingInWmsFinishedBy;
        LoadingInWmsFinishedAt = loadingInWmsFinishedAt;
        CreatedAt = createdAt;
        PrintCopiesCount = printCopiesCount;
        DeliveryDocuments = deliveryDocuments;
        SigningDeviceCode = null;
        SentToSigningDeviceAt = null;
        SentToSigningDeviceByUserName = null;
        FinishedAt = null;     
        FinishedByUserName = null;
        FinishMethodType = null;
        SignedByDriverName = null;
        FinishedByUserFullName = null;
        DeletionReason = null;
        SignedWithLicensePlate = null;
        SentEmails = [];
    }
    
    private LoadingDocument(LoadingDocument original, DeliveryDocument[] filteredDocuments)
    {
        Id = original.Id;
        Code = original.Code;
        GateNumber = original.GateNumber;
        DepositorCode = original.DepositorCode;
        DepositorName = original.DepositorName;
        DeliveryMethodCode = original.DeliveryMethodCode;
        DeliveryMethodName = original.DeliveryMethodName;
        OriginalLicensePlate = original.OriginalLicensePlate;
        OriginalDriverName = original.OriginalDriverName;
        Weight = original.Weight;
        AdrPoints = original.AdrPoints;
        LoadingLocation = original.LoadingLocation;
        LoadingInWmsFinishedBy = original.LoadingInWmsFinishedBy;
        LoadingInWmsFinishedAt = original.LoadingInWmsFinishedAt;
        CreatedAt = original.CreatedAt;
        PrintCopiesCount = original.PrintCopiesCount;
        SigningDeviceCode = original.SigningDeviceCode;
        SentToSigningDeviceAt = original.SentToSigningDeviceAt;
        SentToSigningDeviceByUserName = original.SentToSigningDeviceByUserName;
        FinishedAt = original.FinishedAt;    
        FinishedByUserName = original.FinishedByUserName;
        FinishMethodType = original.FinishMethodType;
        FinishedByUserFullName = original.FinishedByUserFullName;
        DeletionReason = original.DeletionReason;
        SignedByDriverName = original.SignedByDriverName;
        SignedWithLicensePlate = original.SignedWithLicensePlate;
        DeliveryDocuments = filteredDocuments;
        SentEmails = original.SentEmails;
    }
    
    public LoadingDocument WithFilteredDeliveryDocuments(string[] deliveryDocumentCodes)
    {
        var upperDeliveryDocumentCodes = deliveryDocumentCodes.Select(d => d.ToUpperInvariant()).ToArray();
        var filteredDocuments = DeliveryDocuments
            .Where(doc => upperDeliveryDocumentCodes.Contains(doc.Code))
            .ToArray();

        return new LoadingDocument(this, filteredDocuments);
    }
    
    public LoadingDocument WithFilteredAndSignedDeliveryDocuments(string[] deliveryDocumentCodes)
    {
        var upperDeliveryDocumentCodes = deliveryDocumentCodes.Select(d => d.ToUpperInvariant()).ToArray();
        var filteredDocuments = DeliveryDocuments
            .Where(doc => upperDeliveryDocumentCodes.Contains(doc.Code))
            .ToArray();

        var signedDeliveryDocuments = DeliveryDocuments
            .Where(x => x is { IsFinished: true, FinishMethodType: FinishMethod.Signed or FinishMethod.ManuallyUploaded }
                && !upperDeliveryDocumentCodes.Contains(x.Code))
            .ToArray();
        
        var uniqueDocuments = filteredDocuments.Union(signedDeliveryDocuments).ToArray();
        return new LoadingDocument(this, uniqueDocuments);
    }

    public bool IsLoadingDocumentFullyFinishedWithAllDeliveryDocuments()
    {
        return IsFinished && DeliveryDocuments.All(x => x.IsFinished);
    }

    public void SignDocument(bool shouldAlsoSignLoadingDocument, 
        string[] deliveryDocumentCodesToSign, DateTimeOffset signedAt, 
        string signedByUserFullName, string signedByUserName, 
        string driverName, string licensePlate, string? depositorName)
    {
        DepositorName = depositorName;
        
        if (shouldAlsoSignLoadingDocument)
        {
            FinishedAt = signedAt;
            FinishedByUserName = signedByUserName;
            FinishMethodType = FinishMethod.Signed;
            FinishedByUserFullName = signedByUserFullName;
            SignedByDriverName = driverName;
            SignedWithLicensePlate = licensePlate;
        }

        foreach (var deliveryDocumentCodeToSign in deliveryDocumentCodesToSign)
        {
            var deliveryDocument = DeliveryDocuments.FirstOrDefault(x => x.Code == deliveryDocumentCodeToSign);
            deliveryDocument?.SignDocument(signedAt, signedByUserFullName, signedByUserName, driverName, licensePlate);
        }
    }
    
    public void DeleteDocument(bool shouldAlsoDeleteLoadingDocument, 
        string[] deliveryDocumentCodesToDelete, DateTimeOffset deletedAt, string? deletionReason,
        string deletedByUserName, string? deletedByUserFullName, string? depositorName)
    {
        DepositorName = depositorName;
        
        if (shouldAlsoDeleteLoadingDocument)
        {
            FinishedAt = deletedAt;
            FinishedByUserName = deletedByUserName;
            FinishMethodType = FinishMethod.Deleted;
            FinishedByUserFullName = deletedByUserFullName;
            DeletionReason = deletionReason;
        }

        foreach (var deliveryDocumentCodeToDelete in deliveryDocumentCodesToDelete)
        {
            var deliveryDocument = DeliveryDocuments.FirstOrDefault(x => x.Code == deliveryDocumentCodeToDelete);
            deliveryDocument?.DeleteDocument(deletedAt, deletedByUserName, deletedByUserFullName, deletionReason);
        }
    }

    public void ManuallySignDocumentWithUploadedImage(bool isLoadingDocumentSigned, string? signedDeliveryDocumentCode, 
        DateTimeOffset signedAt, string signedByUserName, string? signedByUserFullName, string? depositorName)
    {
        DepositorName = depositorName;
        
        if (isLoadingDocumentSigned)
        {
            FinishedAt = signedAt;
            FinishedByUserName = signedByUserName;
            FinishMethodType = FinishMethod.ManuallyUploaded;
            FinishedByUserFullName = signedByUserFullName;
            return;
        }
        
        var deliveryDocument = DeliveryDocuments.FirstOrDefault(x => x.Code == signedDeliveryDocumentCode);

        deliveryDocument?.ManuallySignDocument(signedAt, signedByUserName, signedByUserFullName);
    }
    
    public ErrorOr<Success> ChangePrintCopiesCount(string? deliveryDocumentCode, int printCopiesCount)
    {
        if (deliveryDocumentCode is not null)
        {
            var deliveryDocument = DeliveryDocuments.FirstOrDefault(x => x.Code == deliveryDocumentCode);
            if (deliveryDocument is null)
                return LoadingDocumentDomainErrors.ValidationDeliveryDocumentDoesNotExistInLoadingDocument(Code, deliveryDocumentCode);

            return deliveryDocument.ChangePrintCopiesCount(printCopiesCount);
        }
            
        if (IsFinished)
            return LoadingDocumentDomainErrors.ValidationLoadingDocumentIsAlreadyFinished(Code);

        PrintCopiesCount = printCopiesCount;
        
        return Result.Success;
    }

    public ErrorOr<Success> CanLoadingDocumentBeSigned()
    {
        if (SentToSigningDeviceAt is null)
            return LoadingDocumentDomainErrors.ValidationLoadingDocumentIsNotSentToSigningDevice(Code);
            
        if (IsFinished)
            return LoadingDocumentDomainErrors.ValidationLoadingDocumentIsAlreadyFinished(Code);

        return Result.Success;
    }

    public ErrorOr<Success> CanDeliveryDocumentsBeSigned(string[] deliveryDocumentCodes)
    {
        if (deliveryDocumentCodes.Length == 0)
            return Result.Success;
        
        var deliveryDocuments = GetAllDeliveryDocuments(deliveryDocumentCodes);
        if (deliveryDocuments.IsError)
            return deliveryDocuments.Errors;
        
        foreach (var deliveryDocument in deliveryDocuments.Value)
        {
            if (deliveryDocument.SigningDeviceCode is null)
                return LoadingDocumentDomainErrors.ValidationDeliveryDocumentIsNotSentToSigningDevice(Code, deliveryDocument.Code);
            
            if (deliveryDocument.IsFinished)
                return LoadingDocumentDomainErrors.ValidationDeliveryDocumentIsAlreadyFinished(Code, deliveryDocument.Code);
        }
        
        return Result.Success;
    }

    public ErrorOr<Success> CanDocumentBeDownloaded(string? deliveryDocumentCode)
    {
        if (string.IsNullOrWhiteSpace(deliveryDocumentCode))
        {
            if (!IsFinished)
                return LoadingDocumentDomainErrors.ValidationLoadingDocumentIsNotFinished(Code);

            return Result.Success;
        }
        
        var deliveryDocument = DeliveryDocuments.FirstOrDefault(x => x.Code == deliveryDocumentCode);
        if (deliveryDocument is null)
            return LoadingDocumentDomainErrors.ValidationDeliveryDocumentDoesNotExistInLoadingDocument(Code, deliveryDocumentCode);
        
        if (!deliveryDocument.IsFinished)
            return LoadingDocumentDomainErrors.ValidationDeliveryDocumentIsNotFinished(Code, deliveryDocument.Code);
        
        return Result.Success;
    }

    public ErrorOr<Success> CanDocumentBeManuallySigned(string? deliveryDocumentCode)
    {
        if (string.IsNullOrWhiteSpace(deliveryDocumentCode))
        {
            if (IsFinished)
                return LoadingDocumentDomainErrors.ValidationLoadingDocumentIsAlreadyFinished(Code);
            
            if (SentToSigningDeviceAt is not null)
                return LoadingDocumentDomainErrors.ValidationLoadingDocumentAlreadySentToSigningDevice(Code);
            
            return Result.Success;
        }
        
        var deliveryDocument = DeliveryDocuments.FirstOrDefault(x => x.Code == deliveryDocumentCode);
        if (deliveryDocument is null)
            return LoadingDocumentDomainErrors.ValidationDeliveryDocumentDoesNotExistInLoadingDocument(Code, deliveryDocumentCode);
        
        if (deliveryDocument.SentToSigningDeviceAt is not null)
            return LoadingDocumentDomainErrors.ValidationDeliveryDocumentAlreadySentToSigningDevice(Code, deliveryDocument.Code);
        
        if (deliveryDocument.IsFinished)
            return LoadingDocumentDomainErrors.ValidationDeliveryDocumentIsAlreadyFinished(Code, deliveryDocument.Code);
        
        return Result.Success;
    }

    public ErrorOr<Success> CanLoadingDocumentBeSentToSigningDevice()
    {
        if (IsFinished)
            return LoadingDocumentDomainErrors.ValidationLoadingDocumentIsAlreadyFinished(Code);
        
        if (SigningDeviceCode is not null)
            return LoadingDocumentDomainErrors.ValidationLoadingDocumentAlreadySentToSigningDevice(Code);
        
        return Result.Success;
    }

    public ErrorOr<Success> CanDeliveryDocumentsBeSentToSigningDevice(string[] deliveryDocumentCodes)
    {
        if (deliveryDocumentCodes.Length == 0)
            return Result.Success;

        var deliveryDocuments = GetAllDeliveryDocuments(deliveryDocumentCodes);
        if (deliveryDocuments.IsError)
            return deliveryDocuments.Errors;
        
        foreach (var deliveryDocument in deliveryDocuments.Value)
        {
            if (deliveryDocument.SigningDeviceCode is not null)
                return LoadingDocumentDomainErrors.ValidationDeliveryDocumentAlreadySentToSigningDevice(Code, deliveryDocument.Code);
            
            if (deliveryDocument.IsFinished)
                return LoadingDocumentDomainErrors.ValidationDeliveryDocumentIsAlreadyFinished(Code, deliveryDocument.Code);
        }
        
        return Result.Success;
    }

    public ErrorOr<Success> CanLoadingDocumentWithDeliveryDocumentsBeDeleted(bool shouldLoadingDocumentBeAlsoDeleted, string[] deliveryDocumentCodes)
    {
        if (shouldLoadingDocumentBeAlsoDeleted && IsFinished)
            return LoadingDocumentDomainErrors.ValidationLoadingDocumentIsAlreadyFinished(Code);
        
        if (deliveryDocumentCodes.Length == 0)
            return Result.Success;
        
        var deliveryDocuments = GetAllDeliveryDocuments(deliveryDocumentCodes);
        if (deliveryDocuments.IsError)
            return deliveryDocuments.Errors;
        
        foreach (var deliveryDocument in deliveryDocuments.Value)
        {
            if (deliveryDocument.SigningDeviceCode is not null)
                return LoadingDocumentDomainErrors.ValidationDeliveryDocumentAlreadySentToSigningDevice(Code, deliveryDocument.Code);
            
            if (deliveryDocument.IsFinished)
                return LoadingDocumentDomainErrors.ValidationDeliveryDocumentIsAlreadyFinished(Code, deliveryDocument.Code);
        }
        
        return Result.Success;
    }

    public string GetEmailMetadata(string? deliveryDocumentCode)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"Kód nakládkového listu: {Code}");
        
        if (string.IsNullOrWhiteSpace(deliveryDocumentCode))
            return stringBuilder.ToString();
        
        var deliveryDocument = DeliveryDocuments.FirstOrDefault(x => x.Code == deliveryDocumentCode);
        if (deliveryDocument is null)
            return stringBuilder.ToString();
        
        stringBuilder.AppendLine();
        stringBuilder.AppendLine($"Kód dodacího listu: {deliveryDocument.Code}");
        
        stringBuilder.AppendLine();
        stringBuilder.AppendLine($"Číslo objednávky: {deliveryDocument.PartnersOrderNumber}");
        
        return stringBuilder.ToString();
    }

    private ErrorOr<DeliveryDocument[]> GetAllDeliveryDocuments(string[] deliveryDocumentCodes)
    {
        var deliveryDocuments = DeliveryDocuments.Where(x => deliveryDocumentCodes.Contains(x.Code)).ToArray();
        if (deliveryDocuments.Length != deliveryDocumentCodes.Length)
        {
            var missingDocumentCodes = deliveryDocumentCodes.Except(DeliveryDocuments.Select(x => x.Code)).ToArray();
            var missingDocumentsCommaSeparated = string.Join(", ", missingDocumentCodes);
            return LoadingDocumentDomainErrors.ValidationSomeDeliveryDocumentsDoNotExists(missingDocumentsCommaSeparated);
        }
        
        return deliveryDocuments;
    }

    private void AddSentEmail(Guid emailId, string[] recipients, DateTimeOffset processedAt, int attachmentsCount, string? failureReason)
    {
        SentEmails.Add(new SentEmail(emailId, recipients, processedAt, attachmentsCount, failureReason));
    }

    public void Apply(LoadingDocumentCreatedEvent @event)
    {
        Code = @event.Code;
        GateNumber = @event.GateNumber;
        DepositorCode = @event.DepositorCode;
        DepositorName = @event.DepositorName;
        DeliveryMethodCode = @event.DeliveryMethodCode;
        OriginalLicensePlate = @event.LicensePlate;
        OriginalDriverName = @event.DriverName;
        Weight = @event.Weight;
        AdrPoints = @event.AdrPoints;
        CreatedAt = @event.CreatedAt;
        LoadingLocation = @event.LoadingLocation;
        LoadingInWmsFinishedBy = @event.LoadingInWmsFinishedBy;
        LoadingInWmsFinishedAt = @event.LoadingInWmsFinishedAt;
        PrintCopiesCount = @event.RequestedPrintCopiesCount;
        DeliveryDocuments = @event.DeliveryDocuments;
        SigningDeviceCode = null;
        SentToSigningDeviceAt = null;
        SentToSigningDeviceByUserName = null;
        FinishedAt = null;
        FinishedByUserName = null;
        FinishMethodType = null;
        SignedByDriverName = null;
        SignedWithLicensePlate = null;
    }
    
    public void Apply(LoadingDocumentToSigningDeviceSentEvent @event)
    {
        if (@event.IsLoadingDocumentAlsoSent)
        {
            SigningDeviceCode = @event.SigningDeviceCode;
            SentToSigningDeviceAt = @event.SentAt;
            SentToSigningDeviceByUserName = @event.SentByUserName;
        }
        
        var deliveryDocuments = DeliveryDocuments.Where(x => @event.DeliveryDocumentCodes.Contains(x.Code)).ToArray();
        
        foreach (var deliveryDocument in deliveryDocuments)
        {
            deliveryDocument.SendDeliveryDocument(@event.SigningDeviceCode, @event.SentAt,  @event.SentByUserName);
        }
    }
    
    public void Apply(LoadingDocumentFromSigningDeviceReturnedEvent @event)
    {
        if (@event.IsLoadingDocumentAlsoReturned)
        {
            SigningDeviceCode = null;
            SentToSigningDeviceAt = null;
            SentToSigningDeviceByUserName = null;
        }
        
        var deliveryDocuments = DeliveryDocuments.Where(x => @event.DeliveryDocumentCodes.Contains(x.Code)).ToArray();

        foreach (var deliveryDocument in deliveryDocuments)
        {
            deliveryDocument.ReturnDeliveryDocument();
        }
    }

    public void Apply(SignedDocumentManuallyUploadedEvent @event)
    {
        ManuallySignDocumentWithUploadedImage(@event.IsLoadingDocumentSignature, @event.DeliveryDocumentCode, @event.UploadedAt, @event.UploadedByUserName, @event.UploadedByUserFullName, @event.DepositorName);;
    }
    
    public void Apply(LoadingDocumentSignedEvent @event)
    {
        SignDocument(@event.IsLoadingDocumentAlsoSigned, @event.DeliveryDocumentCodes, 
            @event.SignedAt, @event.SignedByFullName, @event.SignedByUserName, 
            @event.DriverName, @event.LicensePlate, @event.DepositorName);
    }
    
    public void Apply(LoadingDocumentDeletedEvent @event)
    {
        DeleteDocument(@event.IsLoadingDocumentAlsoDeleted, @event.DeliveryDocumentCodes, 
            @event.DeletedAt, @event.DeletionReason, @event.DeletedByUserName, @event.DeletedByUserFullName, @event.DepositorName);
    }
    
    public void Apply(LoadingDocumentEmailSentEvent @event)
    {
        if (string.IsNullOrWhiteSpace(@event.DeliveryDocumentCode))
        {
            AddSentEmail(@event.EmailId, @event.Recipients, @event.ProcessedAt, @event.AttachmentsCount, @event.FailureReason);
            return;
        }
        
        var deliveryDocument = DeliveryDocuments.FirstOrDefault(x => x.Code == @event.DeliveryDocumentCode);

        deliveryDocument?.AddSentEmail(@event.EmailId, @event.Recipients, @event.ProcessedAt, @event.AttachmentsCount, @event.FailureReason);
    }
    
    public void Apply(LoadingDocumentPrintCopiesCountChangedEvent @event)
    {
        ChangePrintCopiesCount(@event.DeliveryDocumentCode, @event.PrintCopiesCount);
    }
}