using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.Settings.Domain.ReceiverAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Receivers.Queries.GetReceiversByPartnerCode;

internal record GetReceiversByPartnerCodeQuery(string PartnerCode) : IRequest<Receiver[]>; 