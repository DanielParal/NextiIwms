using MediatR;
using Nexticz.Module.Sign.Settings.Domain.DepositorAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Depositors.Queries.GetDepositorsByCodes;

internal record GetDepositorsByCodesQuery(string[] Codes) : IRequest<Depositor[]>; 