using MediatR;
using Nexticz.Module.Cuzk.Domain.ImportAggregate;

namespace Nexticz.Module.Cuzk.Application.ImportsExports.Imports.Queries.GetRequestedImports;

internal record GetRequestedImportsQuery() : IRequest<Import[]>;