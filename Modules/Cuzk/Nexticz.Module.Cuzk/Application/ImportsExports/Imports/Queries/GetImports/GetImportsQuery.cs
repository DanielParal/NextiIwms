using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Cuzk.Domain.ImportAggregate;

namespace Nexticz.Module.Cuzk.Application.ImportsExports.Imports.Queries.GetImports;

internal record GetImportsQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<Import>>;