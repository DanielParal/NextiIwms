using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.Settings.Domain.ConstantAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Constants.Queries.GetConstants;

internal record GetConstantsQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<Constant>>;