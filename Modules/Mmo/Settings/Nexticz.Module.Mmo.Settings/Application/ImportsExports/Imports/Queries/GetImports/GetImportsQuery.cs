using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Domain.ImportEntity;


namespace Nexticz.Module.Mmo.Settings.Application.ImportsExports.Imports.Queries.GetImports;

internal record GetImportsQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<Import>>;