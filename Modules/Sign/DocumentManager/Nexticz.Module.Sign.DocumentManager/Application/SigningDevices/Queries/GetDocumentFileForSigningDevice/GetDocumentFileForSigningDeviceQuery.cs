using MediatR;
using ErrorOr;
using Nexticz.Lib.Shared.FileHandling.Models;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Queries.GetDocumentFileForSigningDevice;

internal record GetDocumentFileForSigningDeviceQuery(string SigningDeviceCode, string DocumentCode) : IRequest<ErrorOr<FileResult>>;