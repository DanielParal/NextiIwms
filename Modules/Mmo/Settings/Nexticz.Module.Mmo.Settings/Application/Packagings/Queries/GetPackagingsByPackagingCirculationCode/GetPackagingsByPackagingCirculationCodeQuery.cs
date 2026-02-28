using MediatR;
using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Packagings.Queries.GetPackagingsByPackagingCirculationCode;

internal record GetPackagingsByPackagingCirculationCodeQuery(string PackagingCirculationCode) : IRequest<IReadOnlyList<Packaging>>;