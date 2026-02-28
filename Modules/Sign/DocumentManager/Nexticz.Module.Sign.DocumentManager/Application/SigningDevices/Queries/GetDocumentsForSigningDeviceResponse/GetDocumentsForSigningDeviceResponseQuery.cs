using MediatR;
using ErrorOr;
using Nexticz.Module.Sign.DocumentManager.Contracts.SigningDevices;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Queries.GetDocumentsForSigningDeviceResponse;

internal record GetDocumentsForSigningDeviceResponseQuery(string SigningDeviceCode) : IRequest<ErrorOr<DocumentsForSigningDeviceResponse>>;