using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using MimeKit;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.EmailSender.Application.Interfaces;

namespace Nexticz.Module.EmailSender.Infrastructure.Smtp;

internal class SmtpEmailSender(
    SmtpSettings smtpSettings,
    ILogger<SmtpEmailSender> logger,
    IFeatureManager featureManager,
    IClock clock) : IEmailSender, IDisposable
{
    private readonly SmtpClient _smtpClient = new();
    private readonly TimeSpan _smtpClientTimeout = TimeSpan.FromSeconds(10);
    private DateTimeOffset _disconnectAfter = DateTimeOffset.MinValue;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private const string EmailSenderSendEmails = nameof(EmailSenderSendEmails);
    
    public async Task<string> SendAsync(MimeMessage message, CancellationToken token)
    {
        string result;
        await _semaphore.WaitAsync(token);
        try {
            await ConnectAsync(token);

            if (await featureManager.IsEnabledAsync(nameof(EmailSenderSendEmails)))
            {
                result = await _smtpClient.SendAsync(message, token);
            }
            else
            {
                logger.LogWarning("EmailSender - Sending emails is disabled with feature flag.");
                result = "Sending emails is disabled with feature flag.";
            }
        } catch (Exception ex) {
            logger.LogError(ex, "EmailSender - error while sending email");
            throw;
        } finally {
            _semaphore.Release();
        }
        
        logger.LogTrace("EmailSender - Smtp - email sent with result: {result}", result);
        return result;
    }
    
    private async Task ConnectAsync(CancellationToken token) {
        if (_smtpClient.IsConnected) {
            await _smtpClient.NoOpAsync(token);
            logger.LogTrace("Reusing SMTP connection for {host}", smtpSettings.Host);
        } else {
            logger.LogTrace("Connecting SMTP client to {host}", smtpSettings.Host);
            await _smtpClient.ConnectAsync(
                smtpSettings.Host, 
                smtpSettings.Port,
                smtpSettings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None,
                token);
            
            if (!string.IsNullOrWhiteSpace(smtpSettings.Username) || !string.IsNullOrWhiteSpace(smtpSettings.Password))
                await _smtpClient.AuthenticateAsync(smtpSettings.Username, smtpSettings.Password, token);
        }
        ScheduleDisconnect(token);
    }
    
    private void ScheduleDisconnect(CancellationToken token) {
        _disconnectAfter = clock.UtcNowOffset.Add(_smtpClientTimeout);
        Task.Run(async () => {
            await Task.Delay(_smtpClientTimeout.Add(TimeSpan.FromSeconds(1)), token);
            if (clock.UtcNowOffset > _disconnectAfter) 
                await DisconnectAsync(token);
        }, token);
    }
    
    private async Task DisconnectAsync(CancellationToken token) {
        try {
            if (!_smtpClient.IsConnected) 
                return;
            
            logger.LogTrace("Disconnecting SMTP client");
            await _smtpClient.DisconnectAsync(true, token);
            _disconnectAfter = DateTimeOffset.MinValue;
        } catch (Exception ex) {
            logger.LogError(ex, "EmailSender - error while disconnecting SMTP client");
            throw;
        }
    }

    public void Dispose()
    {
        _smtpClient.DisconnectAsync(true);
        _smtpClient.Dispose();
    }
}