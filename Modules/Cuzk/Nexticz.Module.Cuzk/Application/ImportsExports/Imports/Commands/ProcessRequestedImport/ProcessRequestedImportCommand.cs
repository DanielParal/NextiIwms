using ErrorOr;
using Nexticz.Module.Cuzk.Domain.ImportAggregate;

namespace Nexticz.Module.Cuzk.Application.ImportsExports.Imports.Commands.ProcessRequestedImport;

internal record ProcessRequestedImportCommand(Import Import, string RoundKey) : ICuzkCommand<ErrorOr<Success>>;