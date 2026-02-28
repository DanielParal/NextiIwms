using MediatR;

namespace Nexticz.Module.Sign.Settings.Contracts.Depositors.Queries;

public record GetDepositorGroupCodesByCodesQuery(string[] Codes) : IRequest<string[]>;