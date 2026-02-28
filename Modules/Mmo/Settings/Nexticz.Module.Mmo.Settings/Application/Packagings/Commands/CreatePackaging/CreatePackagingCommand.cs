using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Settings.Contracts.Packagings;

namespace Nexticz.Module.Mmo.Settings.Application.Packagings.Commands.CreatePackaging;

internal record CreatePackagingCommand(CreatePackagingRequest CreatePackagingRequest) : ISettingsCommand<ErrorOr<Domain.PackagingAggregate.Packaging>>;