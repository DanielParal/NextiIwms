using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Domain.ConstantEntity;


namespace Nexticz.Module.Mmo.Settings.Application.Constants.Queries.GetConstants;

internal record GetConstantsQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<Constant>>;