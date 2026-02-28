using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.Settings.Domain.PartnerAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Partners.Queries.GetPartnerByCode;

internal record GetPartnerByCodeQuery(string Code) : IRequest<ErrorOr<Partner>>; 