using MediatR;

namespace Nexticz.Module.Sign.Settings.Contracts.DeliveryMethods.Queries;

public record GetDeliveryMethodContractsByCodesQuery(string[] Codes) : IRequest<DeliveryMethodContract[]>;