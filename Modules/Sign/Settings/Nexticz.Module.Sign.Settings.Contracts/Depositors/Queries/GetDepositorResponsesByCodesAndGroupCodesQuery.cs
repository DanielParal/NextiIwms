using MediatR;

namespace Nexticz.Module.Sign.Settings.Contracts.Depositors.Queries;

public record GetDepositorResponsesByCodesAndGroupCodesQuery(string[] Codes, string[] GroupCodes) : IRequest<DepositorResponse[]>; 
