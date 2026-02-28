using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.DocumentLoader.Contracts;

namespace Nexticz.Module.Sign.DocumentManager.Application.PartnersAndReceivers.Commands;

internal record CreateMissingPartnersAndReceiversCommand(DeliveryDocumentContract[] DeliveryDocuments) : IRequest<Success>;