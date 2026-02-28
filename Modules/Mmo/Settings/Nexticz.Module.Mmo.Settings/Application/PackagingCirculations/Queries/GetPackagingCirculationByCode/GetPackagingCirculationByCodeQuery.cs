using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Settings.Domain.PackagingCirculationEntity;

namespace Nexticz.Module.Mmo.Settings.Application.PackagingCirculations.Queries.GetPackagingCirculationByCode;

internal record GetPackagingCirculationByCodeQuery(string Code) : IRequest<ErrorOr<PackagingCirculation>>;