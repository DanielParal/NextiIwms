using ErrorOr;
using MediatR;

namespace Nexticz.Module.Mmo.Settings.Application.Seeds.Commands.CreateSeed;

internal record CreateSeedCommand() : ISettingsCommand<Success>;