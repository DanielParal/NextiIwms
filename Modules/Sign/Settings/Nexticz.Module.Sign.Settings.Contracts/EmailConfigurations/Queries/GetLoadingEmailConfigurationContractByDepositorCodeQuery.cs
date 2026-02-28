using MediatR;
using ErrorOr;

namespace Nexticz.Module.Sign.Settings.Contracts.EmailConfigurations.Queries;

public record GetLoadingEmailConfigurationContractByDepositorCodeQuery(string DepositorCode) : IRequest<ErrorOr<EmailConfigurationContract>>;