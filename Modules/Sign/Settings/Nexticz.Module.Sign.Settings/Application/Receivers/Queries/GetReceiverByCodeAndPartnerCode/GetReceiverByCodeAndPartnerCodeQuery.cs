using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.Settings.Domain.ReceiverAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Receivers.Queries.GetReceiverByCodeAndPartnerCode;

internal record GetReceiverByCodeAndPartnerCodeQuery(string Code, string PartnerCode) : IRequest<ErrorOr<Receiver>>; 