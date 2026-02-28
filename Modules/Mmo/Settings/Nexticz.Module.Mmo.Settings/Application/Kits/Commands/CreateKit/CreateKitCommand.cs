using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Settings.Contracts.Kits;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.Commands.CreateKit;

internal record CreateKitCommand(CreateKitRequest CreateKitRequest) : ISettingsCommand<ErrorOr<Kit>>;