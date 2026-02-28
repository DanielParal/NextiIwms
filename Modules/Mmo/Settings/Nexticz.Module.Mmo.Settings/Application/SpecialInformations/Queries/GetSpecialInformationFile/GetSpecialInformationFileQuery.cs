using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Settings.Application.FileHandling;

namespace Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Queries.GetSpecialInformationFile;

internal record GetSpecialInformationFileQuery(Guid Id) : IRequest<ErrorOr<SpecialInformationFileResult>>;