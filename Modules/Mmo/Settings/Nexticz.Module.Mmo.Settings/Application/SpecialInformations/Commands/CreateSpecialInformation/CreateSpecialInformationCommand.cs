using ErrorOr;
using Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity;

namespace Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Commands.CreateSpecialInformation;

internal record CreateSpecialInformationCommand(string Title, string Description) : ISettingsCommand<ErrorOr<SpecialInformation>>;