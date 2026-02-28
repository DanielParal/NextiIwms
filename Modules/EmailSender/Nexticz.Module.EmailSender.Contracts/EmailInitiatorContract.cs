namespace Nexticz.Module.EmailSender.Contracts;

public record EmailInitiatorContract(
    string ModuleName, 
    string EmailType, 
    bool ShouldSendConfirmationMessage, 
    Dictionary<string, string> InitiatorProperties);