using ErrorOr;
using MediatR;

namespace Nexticz.Module.Sign.Settings.Contracts.Depositors.Queries;

public record GetDepositorResponseByCodeQuery(string Code) : IRequest<ErrorOr<DepositorResponse>>;