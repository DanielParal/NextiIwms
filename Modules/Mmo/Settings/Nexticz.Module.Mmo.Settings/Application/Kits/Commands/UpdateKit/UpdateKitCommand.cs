using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Settings.Contracts.Kits;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.Commands.UpdateKit;

internal record UpdateKitCommand(string Code, UpdateKitRequest UpdateKitRequest) : ISettingsCommand<ErrorOr<Updated>>;