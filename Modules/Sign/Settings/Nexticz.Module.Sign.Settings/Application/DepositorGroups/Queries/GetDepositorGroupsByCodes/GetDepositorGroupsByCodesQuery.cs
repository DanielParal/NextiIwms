using MediatR;
using Nexticz.Module.Sign.Settings.Domain.DepositorGroupAggregate;

namespace Nexticz.Module.Sign.Settings.Application.DepositorGroups.Queries.GetDepositorGroupsByCodes;

internal record GetDepositorGroupsByCodesQuery(string[] Codes) : IRequest<DepositorGroup[]>; 