using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Settings.Domain.ManufactureEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Manufactures.Queries.GetManufactureByCode;

internal record GetManufactureByCodeQuery(string Code) : IRequest<ErrorOr<Manufacture>>;