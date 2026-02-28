using ErrorOr;
using MediatR;

namespace Nexticz.Module.Mmo.Settings.Contracts.Depositors.Queries;

public record GetDepositorResponseByCodeQuery(string Code) : IRequest<ErrorOr<DepositorResponse>>;