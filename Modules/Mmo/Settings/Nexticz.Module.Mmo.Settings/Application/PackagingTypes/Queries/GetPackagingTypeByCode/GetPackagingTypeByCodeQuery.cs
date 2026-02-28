using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Settings.Domain.PackagingTypeEntity;

namespace Nexticz.Module.Mmo.Settings.Application.PackagingTypes.Queries.GetPackagingTypeByCode;

internal record GetPackagingTypeByCodeQuery(string Code) : IRequest<ErrorOr<PackagingType>>;