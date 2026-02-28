using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.DocumentManager.Domain.Views;

namespace Nexticz.Module.Sign.DocumentManager.Application.Emails.Queries.GetSentEmails;

internal record GetSentEmailsQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<SentEmailView>>;