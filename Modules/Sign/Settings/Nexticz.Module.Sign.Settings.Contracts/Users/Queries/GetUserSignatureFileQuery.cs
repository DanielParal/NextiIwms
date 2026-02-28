using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.FileHandling.Models;

namespace Nexticz.Module.Sign.Settings.Contracts.Users.Queries;

public record GetUserSignatureFileQuery(string UserName) : IRequest<ErrorOr<FileResult>>;