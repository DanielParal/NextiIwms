namespace Nexticz.Module.EmailSender.Infrastructure.Smtp;

internal class SmtpSettings
{
    public string Host { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int Port { get; set; }
    public bool UseSsl { get; set; }
}