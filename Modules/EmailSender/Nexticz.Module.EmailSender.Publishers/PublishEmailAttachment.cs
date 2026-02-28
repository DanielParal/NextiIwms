namespace Nexticz.Module.EmailSender.Publishers;

public record PublishEmailAttachment(
    string FileName, string NewFileName, string FilePath, string ContentType);