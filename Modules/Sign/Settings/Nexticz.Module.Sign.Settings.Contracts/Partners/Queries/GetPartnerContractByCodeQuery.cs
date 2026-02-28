using ErrorOr;
using MediatR;

namespace Nexticz.Module.Sign.Settings.Contracts.Partners.Queries;

public record GetPartnerContractByCodeQuery(string Code) : IRequest<ErrorOr<PartnerContract>>;