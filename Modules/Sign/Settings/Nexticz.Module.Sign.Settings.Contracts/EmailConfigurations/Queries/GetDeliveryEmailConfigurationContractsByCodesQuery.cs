using MediatR;

namespace Nexticz.Module.Sign.Settings.Contracts.EmailConfigurations.Queries;

public record GetDeliveryEmailConfigurationContractsByCodesQuery(string DepositorCode, string PartnerCode, string ReceiverCode) : IRequest<EmailConfigurationContract[]>;