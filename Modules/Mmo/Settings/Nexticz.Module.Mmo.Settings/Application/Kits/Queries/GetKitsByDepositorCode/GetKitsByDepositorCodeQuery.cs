using MediatR;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitsByDepositorCode;

internal record GetKitsByDepositorCodeQuery(string DepositorCode) : IRequest<IReadOnlyList<Kit>>;