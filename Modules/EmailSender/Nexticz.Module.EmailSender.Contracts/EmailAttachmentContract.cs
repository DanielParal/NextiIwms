namespace Nexticz.Module.EmailSender.Contracts;

public record EmailAttachmentContract(
    string FileName, string FilePath, string ContentType);