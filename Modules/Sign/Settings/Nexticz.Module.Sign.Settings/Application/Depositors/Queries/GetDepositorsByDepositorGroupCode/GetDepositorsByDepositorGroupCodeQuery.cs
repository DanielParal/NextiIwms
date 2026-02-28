using MediatR;
using Nexticz.Module.Sign.Settings.Domain.DepositorAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Depositors.Queries.GetDepositorsByDepositorGroupCode;

internal record GetDepositorsByDepositorGroupCodeQuery(string DepositorGroupCode) : IRequest<Depositor[]>; 