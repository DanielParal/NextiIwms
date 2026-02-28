using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Settings.Domain.KitTypeEntity;

namespace Nexticz.Module.Mmo.Settings.Application.KitTypes.Queries.GetKitTypeByCode;

internal record GetKitTypeByCodeQuery(string Code) : IRequest<ErrorOr<KitType>>;