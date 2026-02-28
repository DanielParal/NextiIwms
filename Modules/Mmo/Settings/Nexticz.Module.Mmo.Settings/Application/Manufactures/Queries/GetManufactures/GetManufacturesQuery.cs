using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Domain.ManufactureEntity;


namespace Nexticz.Module.Mmo.Settings.Application.Manufactures.Queries.GetManufactures;

internal record GetManufacturesQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<Manufacture>>;