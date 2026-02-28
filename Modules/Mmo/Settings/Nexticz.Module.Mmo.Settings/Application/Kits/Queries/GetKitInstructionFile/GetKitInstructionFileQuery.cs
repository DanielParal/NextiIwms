using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Settings.Application.FileHandling;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitInstructionFile;

internal record GetKitInstructionFileQuery(string KitCode) : IRequest<ErrorOr<KitInstructionResult>>;