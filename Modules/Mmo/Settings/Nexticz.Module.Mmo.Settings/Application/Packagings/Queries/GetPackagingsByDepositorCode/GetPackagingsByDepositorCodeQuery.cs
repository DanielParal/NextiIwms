using MediatR;
using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Packagings.Queries.GetPackagingsByDepositorCode;

internal record GetPackagingsByDepositorCodeQuery(string DepositorCode) : IRequest<IReadOnlyList<Packaging>>;