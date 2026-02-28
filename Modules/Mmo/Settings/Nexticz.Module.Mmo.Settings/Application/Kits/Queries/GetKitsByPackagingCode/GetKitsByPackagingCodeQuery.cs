using MediatR;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitsByPackagingCode;

internal record GetKitsByPackagingCodeQuery(string PackagingCode) : IRequest<IReadOnlyList<Kit>>;