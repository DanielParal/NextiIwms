using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Settings.Application.Seeds.SeedHandlers;

namespace Nexticz.Module.Mmo.Settings.Application.Seeds.Commands.CreateSeed;

internal class CreateSeedCommandHandler(IEnumerable<ISeedRunner> seedRunners) : IRequestHandler<CreateSeedCommand, Success>
{
    public async Task<Success> Handle(CreateSeedCommand request, CancellationToken cancellationToken)
    {
        foreach (var seedRunner in seedRunners)
        {
            await seedRunner.RunSeedAsync(cancellationToken);
        }
        
        return Result.Success;
    }
}