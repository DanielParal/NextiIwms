using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.Settings.Domain.DepositorAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Depositors.Queries.GetDepositorByCode;

internal record GetDepositorByCodeQuery(string Code) : IRequest<ErrorOr<Depositor>>; 