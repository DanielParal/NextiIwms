using MediatR;

namespace Nexticz.Module.Sign.Settings.Contracts.Receivers.Queries;

public record GetReceiverContractsByNameQuery(string Name) : IRequest<ReceiverContract[]>;