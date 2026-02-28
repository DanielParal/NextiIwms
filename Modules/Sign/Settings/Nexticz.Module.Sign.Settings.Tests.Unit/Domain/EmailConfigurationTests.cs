using Nexticz.Module.Sign.Settings.Domain.EmailConfigurationAggregate;
using Shouldly;

namespace Nexticz.Module.Sign.Settings.Tests.Unit.Domain;

public class EmailConfigurationTests
{
    [Fact]
    public void Create_ValidInput_ReturnsEmailConfiguration()
    {
        var depositorCodes = new[] { "DEP001" };
        var recipientEmailAddresses = new[] { "test@example.com" };
        var shouldSendDeliveryDocument = true;
        var shouldSendLoadingDocument = true;
        var shouldSendImmediately = true;

        var result = EmailConfiguration.CreateNew(
            depositorCodes, [], [], shouldSendDeliveryDocument, shouldSendLoadingDocument, 
            recipientEmailAddresses, shouldSendImmediately);

        result.IsError.ShouldBeFalse();
        var config = result.Value;
        config.DepositorCodes.ShouldBe(depositorCodes);
        config.DepositorCodesInString.ShouldBe(string.Join(",", depositorCodes));
        config.PartnerCodes.ShouldBeEmpty();
        config.ReceiverAndPartnerCombinationCodes.ShouldBeEmpty();
        config.ShouldSendDeliveryDocument.ShouldBeTrue();
        config.ShouldSendLoadingDocument.ShouldBeTrue();
        config.RecipientEmailAddresses.ShouldBe(recipientEmailAddresses);
        config.ShouldSendImmediately.ShouldBeTrue();
        config.Type.ShouldBe(EmailConfigurationType.LoadingConfiguration);
    }

    [Fact]
    public void Create_EmptyCodesCombination_ReturnsError()
    {
        string[] depositorCodes = [];
        string[] partnerCodes = [];
        string[] receiverCodes = [];
        var recipientEmailAddresses = new[] { "test@example.com" };

        var result = EmailConfiguration.CreateNew(
            depositorCodes, partnerCodes, receiverCodes, true, false, 
            recipientEmailAddresses, true);

        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(EmailConfigurationDomainErrors.ValidationInvalidCodesCombination);
    }
    
    [Fact]
    public void Create_InvalidLoadingCodesCombination_ReturnsError()
    {
        string[] depositorCodes = [Guid.NewGuid().ToString()];
        string[] partnerCodes = [Guid.NewGuid().ToString()];
        string[] receiverCodes = [];
        var recipientEmailAddresses = new[] { "test@example.com" };

        var result = EmailConfiguration.CreateNew(
            depositorCodes, partnerCodes, receiverCodes, true, true, 
            recipientEmailAddresses, true);

        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(EmailConfigurationDomainErrors.ValidationDeliveryDocumentCannotSendLoadingDocument);
    }
    
    [Fact]
    public void Create_InvalidDeliveryCodesCombination_ReturnsError()
    {
        string[] depositorCodes = [Guid.NewGuid().ToString()];
        string[] partnerCodes = [];
        string[] receiverCodes = [];
        var recipientEmailAddresses = new[] { "test@example.com" };

        var result = EmailConfiguration.CreateNew(
            depositorCodes, partnerCodes, receiverCodes, true, false, 
            recipientEmailAddresses, true);

        result.IsError.ShouldBeFalse();
        var config = result.Value;
        var upperDepositorCodes = depositorCodes.Select(x => x.ToUpperInvariant()).ToArray();
        config.DepositorCodes.ShouldBe(upperDepositorCodes);
        config.DepositorCodesInString.ShouldBe(string.Join(",", upperDepositorCodes));
        config.PartnerCodes.ShouldBeEmpty();
        config.PartnerCodesInString.ShouldBeEmpty();
        config.ReceiverAndPartnerCombinationCodes.ShouldBeEmpty();
        config.ReceiverAndPartnerCombinationCodesInString.ShouldBeEmpty();
        config.ShouldSendDeliveryDocument.ShouldBeTrue();
        config.ShouldSendLoadingDocument.ShouldBeFalse();
        config.RecipientEmailAddresses.ShouldBe(recipientEmailAddresses);
        config.ShouldSendImmediately.ShouldBeTrue();
        config.Type.ShouldBe(EmailConfigurationType.DeliveryConfiguration);
    }
    
    [Fact]
    public void Create_DeliveryConfigurationCannotSendLoadingDocument_ReturnsError()
    {
        string[] depositorCodes = ["DEP1"];
        string[] partnerCodes = ["PART001"];
        string[] receiverCodes = ["REC1"];
        var shouldSendLoadingDocument = true;
        var recipientEmailAddresses = new[] { "test@example.com" };

        var result = EmailConfiguration.CreateNew(
            depositorCodes, partnerCodes, receiverCodes, true, shouldSendLoadingDocument, 
            recipientEmailAddresses, true);

        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(EmailConfigurationDomainErrors.ValidationDeliveryDocumentCannotSendLoadingDocument);
    }

    [Fact]
    public void Create_EmptyRecipientEmailAddresses_ReturnsError()
    {
        string[] depositorCodes = ["DEP001"];
        var recipientEmailAddresses = Array.Empty<string>();

        var result = EmailConfiguration.CreateNew(
            depositorCodes, [], [], true, true, 
            recipientEmailAddresses, true);

        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(EmailConfigurationDomainErrors.ValidationAtLeastOneEmailAddressMustBeFilledInd);
    }

    [Fact]
    public void Create_InvalidEmailAddress_ReturnsError()
    {
        string[] depositorCodes = ["DEP001"];
        var recipientEmailAddresses = new[] { "test@example.com", "invalid-email" };

        var result = EmailConfiguration.CreateNew(
            depositorCodes, [], [], true, true, 
            recipientEmailAddresses, true);

        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(EmailConfigurationDomainErrors.ValidationInvalidEmailAddress("invalid-email"));
    }
    
    
    // Update uses the same validation as creation. We just need to make sure that the validation is in place with one check.
    [Fact]
    public void Update_EmptyCodesCombination_ReturnsError()
    {
        var emailConfiguration = EmailConfiguration.CreateNew(
            ["DEP001"], [], [], true, true, 
            ["test@example.com"], true);

        string[] depositorCodes = [];
        string[] partnerCodes = [];
        string[] receiverCodes = [];
        var recipientEmailAddresses = new[] { "test@example.com" };

        var result = emailConfiguration.Value.Update(
            depositorCodes, partnerCodes, receiverCodes, true, false, 
            recipientEmailAddresses, true);

        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(EmailConfigurationDomainErrors.ValidationInvalidCodesCombination);
    }
}