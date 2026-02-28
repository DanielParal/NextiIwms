using MediatR;
using ErrorOr;
using Nexticz.Module.Sign.Settings.Domain.DepositorGroupAggregate;

namespace Nexticz.Module.Sign.Settings.Application.DepositorGroups.Queries.GetDepositorGroupByCode;

internal record GetDepositorGroupByCodeQuery(string Code) : IRequest<ErrorOr<DepositorGroup>>; 