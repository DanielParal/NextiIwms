using Nexticz.Module.Sign.DocumentManager.Contracts.Emails;
using Nexticz.Module.Sign.DocumentManager.Domain.Views;

namespace Nexticz.Module.Sign.DocumentManager.Presentation.Emails;

internal static class SentEmailResponseFactory
{
    public static SentEmailResponse Create(SentEmailView sentEmail)
    {
        return new SentEmailResponse(
            sentEmail.Id,
            sentEmail.Recipients,
            sentEmail.Metadata,
            sentEmail.AttachmentsCount,
            sentEmail.FailureReason,
            sentEmail.CreatedAt);
    }
}