namespace Nexticz.Module.EmailSender.Contracts;

public static class EmailSenderDirectoryNameProvider
{
    public static string RootAttachmentsFolder(string assetsBaseFolder) => $"{assetsBaseFolder}/EmailSender/Attachments";
}