using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Washing.Application.FileHandling;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Queries.GetKitInstructionFile;

internal record GetKitInstructionFileQuery(string KitCode) : IRequest<ErrorOr<KitInstructionResult>>;