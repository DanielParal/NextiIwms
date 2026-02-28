using MediatR;
using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Packagings.Queries.GetPackagingsByPackagingTypeCode;

internal record GetPackagingsByPackagingTypeCodeQuery(string PackagingTypeCode) : IRequest<IReadOnlyList<Packaging>>;