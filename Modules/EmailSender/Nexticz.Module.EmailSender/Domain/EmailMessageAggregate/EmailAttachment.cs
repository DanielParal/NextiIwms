namespace Nexticz.Module.EmailSender.Domain.EmailMessageAggregate;

public class EmailAttachment
{
    public string FileName { get; private set; }
    public string ContentType { get; private set; }

    public EmailAttachment(string fileName, string contentType)
    {
        FileName = fileName;
        ContentType = contentType;
    }
}