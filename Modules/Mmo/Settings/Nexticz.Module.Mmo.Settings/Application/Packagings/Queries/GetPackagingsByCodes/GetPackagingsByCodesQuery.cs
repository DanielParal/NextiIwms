using MediatR;
using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Packagings.Queries.GetPackagingsByCodes;

internal record GetPackagingsByCodesQuery(string[] Codes) : IRequest<IReadOnlyList<Packaging>>;