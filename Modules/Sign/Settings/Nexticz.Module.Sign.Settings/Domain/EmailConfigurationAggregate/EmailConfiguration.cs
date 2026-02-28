using Nexticz.Lib.Shared.DomainCore;
using ErrorOr;
using Nexticz.Lib.Shared.Emails;
using Nexticz.Module.Sign.Settings.Domain.EmailConfigurationAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Domain.EmailConfigurationAggregate;

public class EmailConfiguration : AggregateRoot
{
    public const string ReceiverAndPartnerCombinationCodesSeparator = "||";
    
    public string[] DepositorCodes { get; private set; }
    public string DepositorCodesInString => string.Join(",", DepositorCodes);
    public string[] PartnerCodes { get; private set; }
    public string PartnerCodesInString => string.Join(",", PartnerCodes);
    public string[] ReceiverAndPartnerCombinationCodes { get; private set; }
    public string ReceiverAndPartnerCombinationCodesInString => string.Join(",", ReceiverAndPartnerCombinationCodes);
    public bool ShouldSendDeliveryDocument { get; private set; }
    public bool ShouldSendLoadingDocument { get; private set; }
    public string[] RecipientEmailAddresses { get; private set; }
    public bool ShouldSendImmediately { get; private set; }
    public EmailConfigurationType Type { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private EmailConfiguration() {}
    
    private EmailConfiguration(
        string[] depositorCodes, 
        string[] partnerCodes, 
        string[] receiverAndPartnerCombinationCodes, 
        bool shouldSendDeliveryDocument, 
        bool shouldSendLoadingDocument, 
        string[] recipientEmailAddresses, 
        bool shouldSendImmediately, 
        EmailConfigurationType type,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        DepositorCodes = depositorCodes.Select(x => x.ToUpperInvariant()).ToArray();
        PartnerCodes = partnerCodes.Select(x => x.ToUpperInvariant()).ToArray();
        ReceiverAndPartnerCombinationCodes = receiverAndPartnerCombinationCodes.Select(x => x.ToUpperInvariant()).ToArray();
        ShouldSendDeliveryDocument = shouldSendDeliveryDocument;
        ShouldSendLoadingDocument = shouldSendLoadingDocument;
        RecipientEmailAddresses = recipientEmailAddresses;
        ShouldSendImmediately = shouldSendImmediately;
        Type = type;
    }

    public static ErrorOr<EmailConfiguration> CreateNew(
        string[] depositorCodes, 
        string[] partnerCodes, 
        string[] receiverAndPartnerCombinationCodes, 
        bool shouldSendDeliveryDocument, 
        bool shouldSendLoadingDocument, 
        string[] recipientEmailAddresses, 
        bool shouldSendImmediately)
    {
        var isValidRequest =
            IsValid(
                depositorCodes, partnerCodes, receiverAndPartnerCombinationCodes,
                shouldSendLoadingDocument, shouldSendDeliveryDocument,
                recipientEmailAddresses);

        if (isValidRequest.IsError)
            return isValidRequest.Errors;
        
        return new EmailConfiguration(
            depositorCodes, partnerCodes, receiverAndPartnerCombinationCodes, 
            shouldSendDeliveryDocument, shouldSendLoadingDocument, recipientEmailAddresses, 
            shouldSendImmediately, GetType(shouldSendLoadingDocument));
    }

    public ErrorOr<Success> Update(
        string[] depositorCodes, 
        string[] partnerCodes, 
        string[] receiverAndPartnerCombinationCodes, 
        bool shouldSendDeliveryDocument, 
        bool shouldSendLoadingDocument, 
        string[] recipientEmailAddresses, 
        bool shouldSendImmediately)
    {
        var isValidRequest =
            IsValid(
                depositorCodes, partnerCodes, receiverAndPartnerCombinationCodes,
                shouldSendLoadingDocument, shouldSendDeliveryDocument,
                recipientEmailAddresses);

        if (isValidRequest.IsError)
            return isValidRequest.Errors;
        
        DepositorCodes = depositorCodes.Select(x => x.ToUpperInvariant()).ToArray();
        PartnerCodes = partnerCodes.Select(x => x.ToUpperInvariant()).ToArray();
        ReceiverAndPartnerCombinationCodes = receiverAndPartnerCombinationCodes.Select(x => x.ToUpperInvariant()).ToArray();
        ShouldSendDeliveryDocument = shouldSendDeliveryDocument;
        ShouldSendLoadingDocument = shouldSendLoadingDocument;
        RecipientEmailAddresses = recipientEmailAddresses;
        ShouldSendImmediately = shouldSendImmediately;
        Type = GetType(shouldSendLoadingDocument);
        
        return Result.Success;
    }

    private static ErrorOr<Success> IsValid(
        string[] depositorCodes, 
        string[] partnerCodes, 
        string[] receiverAndPartnerCombinationCodes, 
        bool shouldSendLoadingDocument, 
        bool shouldSendDeliveryDocument,
        string[] recipientEmailAddresses)
    {
        if (AreAllCodesEmpty(depositorCodes, partnerCodes, receiverAndPartnerCombinationCodes))
            return EmailConfigurationDomainErrors.ValidationInvalidCodesCombination;

        if (!shouldSendDeliveryDocument && !shouldSendLoadingDocument)
            return EmailConfigurationDomainErrors.ValidationAtLeastOneOptionForSendingEmailMustBeFilledIn;

        if (!AreOnlyDepositorCodesFilled(depositorCodes, partnerCodes, receiverAndPartnerCombinationCodes) && shouldSendLoadingDocument)
            return EmailConfigurationDomainErrors.ValidationDeliveryDocumentCannotSendLoadingDocument;
        
        if (recipientEmailAddresses.Length == 0)
            return EmailConfigurationDomainErrors.ValidationAtLeastOneEmailAddressMustBeFilledInd;

        foreach (var email in recipientEmailAddresses)
        {
            if (!EmailValidator.IsValidEmail(email))
                return EmailConfigurationDomainErrors.ValidationInvalidEmailAddress(email);
        }

        return Result.Success;
    }

    private static bool AreAllCodesEmpty(string[] depositorCodes, string[] partnerCodes, string[] receiverCodes)
    {
        return depositorCodes.Length == 0 && 
                 partnerCodes.Length == 0 && 
                 receiverCodes.Length == 0;
    }
    
    private static bool AreOnlyDepositorCodesFilled(string[] depositorCodes, string[] partnerCodes, string[] receiverAndPartnerCombinationCodes)
        => depositorCodes.Length > 0 && 
           partnerCodes.Length == 0 && 
           receiverAndPartnerCombinationCodes.Length == 0;
    
    private static EmailConfigurationType GetType(bool shouldSendLoadingDocument) => 
        shouldSendLoadingDocument ? EmailConfigurationType.LoadingConfiguration : EmailConfigurationType.DeliveryConfiguration;
    
    public void Apply(EmailConfigurationCreatedEvent @event)
    {
        DepositorCodes = string.IsNullOrWhiteSpace(@event.DepositorCode) ? [] : [@event.DepositorCode];
        PartnerCodes = string.IsNullOrWhiteSpace(@event.PartnerCode) ? [] : [@event.PartnerCode];
        ReceiverAndPartnerCombinationCodes = string.IsNullOrWhiteSpace(@event.ReceiverCode) || string.IsNullOrWhiteSpace(@event.PartnerCode) 
            ? [] : [ComposeReceiverAndPartnerCombinationCodes(@event.ReceiverCode, @event.PartnerCode)];
        ShouldSendDeliveryDocument = @event.ShouldSendDeliveryDocument;
        ShouldSendLoadingDocument = @event.ShouldSendLoadingDocument;
        RecipientEmailAddresses = @event.RecipientEmailAddresses;
        ShouldSendImmediately = @event.ShouldSendImmediately;
        Type = @event.Type;
    }
    
    public void Apply(EmailConfigurationUpdatedEvent @event)
    {
        DepositorCodes = string.IsNullOrWhiteSpace(@event.DepositorCode) ? [] : [@event.DepositorCode];
        PartnerCodes = string.IsNullOrWhiteSpace(@event.PartnerCode) ? [] : [@event.PartnerCode];
        ReceiverAndPartnerCombinationCodes = string.IsNullOrWhiteSpace(@event.ReceiverCode) || string.IsNullOrWhiteSpace(@event.PartnerCode) 
            ? [] : [ComposeReceiverAndPartnerCombinationCodes(@event.ReceiverCode, @event.PartnerCode)];
        ShouldSendDeliveryDocument = @event.ShouldSendDeliveryDocument;
        ShouldSendLoadingDocument = @event.ShouldSendLoadingDocument;
        RecipientEmailAddresses = @event.RecipientEmailAddresses;
        ShouldSendImmediately = @event.ShouldSendImmediately;
        Type = @event.Type;
    }
    
    public void Apply(EmailConfigurationCreatedV2Event @event)
    {
        DepositorCodes = @event.DepositorCodes;
        PartnerCodes = @event.PartnerCodes;
        ReceiverAndPartnerCombinationCodes = @event.ReceiverAndPartnerCombinationCodes;
        ShouldSendDeliveryDocument = @event.ShouldSendDeliveryDocument;
        ShouldSendLoadingDocument = @event.ShouldSendLoadingDocument;
        RecipientEmailAddresses = @event.RecipientEmailAddresses;
        ShouldSendImmediately = @event.ShouldSendImmediately;
        Type = @event.Type;
    }
    
    public void Apply(EmailConfigurationUpdatedV2Event @event)
    {
        DepositorCodes = @event.DepositorCodes;
        PartnerCodes = @event.PartnerCodes;
        ReceiverAndPartnerCombinationCodes = @event.ReceiverAndPartnerCombinationCodes;
        ShouldSendDeliveryDocument = @event.ShouldSendDeliveryDocument;
        ShouldSendLoadingDocument = @event.ShouldSendLoadingDocument;
        RecipientEmailAddresses = @event.RecipientEmailAddresses;
        ShouldSendImmediately = @event.ShouldSendImmediately;
        Type = @event.Type;
    }
    
    public static string ComposeReceiverAndPartnerCombinationCodes(string receiverCode, string partnerCode) => 
        $"{receiverCode.ToUpperInvariant()}{ReceiverAndPartnerCombinationCodesSeparator}{partnerCode.ToUpperInvariant()}";
}