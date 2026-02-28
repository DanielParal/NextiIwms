namespace Nexticz.Module.EmailSender.Domain.EmailMessageAggregate;

public class EmailInitiator
{
    public string ModuleName { get; private set; }
    public string EmailType { get; private set; }
    public bool ShouldSendConfirmationMessage { get; private set; }
    public Dictionary<string, string> InitiatorProperties { get; private set; }
    

    public EmailInitiator(string moduleName, string emailType, bool shouldSendConfirmationMessage, Dictionary<string, string>? initiatorProperties = null)
    {
        ModuleName = moduleName;
        EmailType = emailType;
        ShouldSendConfirmationMessage = shouldSendConfirmationMessage;
        InitiatorProperties = initiatorProperties ?? new Dictionary<string, string>();
    }
}