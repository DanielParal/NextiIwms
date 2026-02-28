using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Packagings.Queries.GetPackagingByCode;

internal record GetPackagingByCodeQuery(string Code) : IRequest<ErrorOr<Packaging>>;