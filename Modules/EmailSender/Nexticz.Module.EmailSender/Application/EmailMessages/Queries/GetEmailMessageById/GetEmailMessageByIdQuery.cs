using MediatR;
using ErrorOr;
using Nexticz.Module.EmailSender.Domain.EmailMessageAggregate;

namespace Nexticz.Module.EmailSender.Application.EmailMessages.Queries.GetEmailMessageById;

internal record GetEmailMessageByIdQuery(Guid Id) : IRequest<ErrorOr<EmailMessage>>;