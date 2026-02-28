using MediatR;
using Nexticz.Module.EmailSender.Domain.Views;

namespace Nexticz.Module.EmailSender.Application.ScheduledEmailMessages.Queries.GetScheduledEmailMessagesToSend;

internal record GetScheduledEmailMessagesToSendQuery() : IRequest<ScheduledEmailMessageView[]>;