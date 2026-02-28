using MediatR;
using Nexticz.Module.Mmo.Settings.Domain.PackagingTypeEntity;

namespace Nexticz.Module.Mmo.Settings.Application.PackagingTypes.Queries.GetPackagingTypesByCodes;

internal record GetPackagingTypesByCodesQuery(string[] Codes) : IRequest<IReadOnlyList<PackagingType>>;