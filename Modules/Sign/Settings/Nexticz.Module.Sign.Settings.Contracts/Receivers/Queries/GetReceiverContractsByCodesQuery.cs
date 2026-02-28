using MediatR;

namespace Nexticz.Module.Sign.Settings.Contracts.Receivers.Queries;

public record GetReceiverContractsByCodesQuery((string ReceiverCode, string PartnerCode)[] ReceiverCodeWithPartnerCodes) : IRequest<ReceiverContract[]>;