using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Drying.Application.Kits.Commands.CreateKit;

internal class CreateKitCommandValidator : AbstractValidator<CreateKitCommand>
{
    public CreateKitCommandValidator()
    {
        RuleFor(x => x.LineCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(KitErrors.ValidationLineCodeIsRequired));
        
        RuleFor(x => x.BatchId)
            .NotEmpty()
            .WithState(x => new CustomErrorState(KitErrors.ValidationBatchIdIsRequired));
        
        RuleFor(x => x.KitId)
            .NotEmpty()
            .WithState(x => new CustomErrorState(KitErrors.ValidationKitIdIsRequired));
        
        RuleFor(x => x.KitCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(KitErrors.ValidationKitCodeIsRequired));
    }
}