using ErrorOr;
using Microsoft.AspNetCore.Http;
using Nexticz.Module.Cuzk.Domain.ImportAggregate;

namespace Nexticz.Module.Cuzk.Application.ImportsExports.Imports.Commands.RequestImport;

internal record RequestImportCommand(ImportType ImportType, IFormFile FormFile, string? CsvDelimiter) : ICuzkCommand<ErrorOr<Success>>;