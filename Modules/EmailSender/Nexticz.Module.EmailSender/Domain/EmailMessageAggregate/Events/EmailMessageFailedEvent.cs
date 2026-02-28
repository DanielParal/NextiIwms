namespace Nexticz.Module.EmailSender.Domain.EmailMessageAggregate.Events;

public record EmailMessageFailedEvent(
    Guid Id, DateTimeOffset FailedAt, string ErrorMessage);