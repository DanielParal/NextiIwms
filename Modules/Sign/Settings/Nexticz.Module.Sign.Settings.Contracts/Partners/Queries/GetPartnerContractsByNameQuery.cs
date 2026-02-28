using MediatR;

namespace Nexticz.Module.Sign.Settings.Contracts.Partners.Queries;

public record GetPartnerContractsByNameQuery(string Name) : IRequest<PartnerContract[]>;