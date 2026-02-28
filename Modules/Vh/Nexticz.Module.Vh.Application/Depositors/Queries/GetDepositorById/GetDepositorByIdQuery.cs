using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.Depositors;

namespace Nexticz.Module.Vh.Application.Depositors.Queries.GetDepositorById;

public class GetDepositorByIdQuery : IRequest<ErrorOr<DepositorResponse>>
{
    public required Guid Id { get; set; }
}