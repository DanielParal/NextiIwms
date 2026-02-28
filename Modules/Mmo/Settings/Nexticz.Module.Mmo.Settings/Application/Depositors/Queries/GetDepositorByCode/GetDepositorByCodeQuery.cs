using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Settings.Domain.DepositorEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Depositors.Queries.GetDepositorByCode;

internal record GetDepositorByCodeQuery(string Code) : IRequest<ErrorOr<Domain.DepositorEntity.Depositor>>; 