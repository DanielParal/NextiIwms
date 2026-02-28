using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Washing.Application.FileHandling;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Queries.GetSpecialInformationFile;

internal record GetSpecialInformationFileQuery(Guid BatchId) : IRequest<ErrorOr<SpecialInformationFileResult>>;