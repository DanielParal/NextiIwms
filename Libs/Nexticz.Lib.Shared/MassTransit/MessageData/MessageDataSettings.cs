namespace Nexticz.Lib.Shared.MassTransit.MessageData;

internal class MessageDataSettings
{
    public const string SectionKey = "MasstransitMessageDataSettings";

    public string BaseFolder { get; set; }
    public TimeSpan TimeToLive { get; set; }
}