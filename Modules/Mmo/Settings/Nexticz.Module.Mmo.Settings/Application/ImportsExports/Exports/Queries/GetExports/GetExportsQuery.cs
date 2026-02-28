using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Domain.ExportEntity;


namespace Nexticz.Module.Mmo.Settings.Application.ImportsExports.Exports.Queries.GetExports;

internal record GetExportsQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<Export>>;