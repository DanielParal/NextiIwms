using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.Settings.Domain.ReceiverAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Receivers.Queries.GetReceivers;

internal record GetReceiversQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<Receiver>>;