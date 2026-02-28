namespace Nexticz.Lib.Shared.Ftps;

public class FtpSettings
{
    public string Host { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
    public int Port { get; set; }
    public bool UseImplicitEncryptionMode { get; set; }
    public bool ShouldValidateSslFingerprint { get; set; }
    public string? SslFingerprint { get; set; }
    public string BasePath { get; set; }
}