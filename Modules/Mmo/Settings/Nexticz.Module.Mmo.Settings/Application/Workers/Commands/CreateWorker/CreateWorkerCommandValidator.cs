using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.Workers.Commands.CreateWorker;

internal class CreateWorkerCommandValidator : AbstractValidator<CreateWorkerCommand>
{
    public CreateWorkerCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithState(x => new CustomErrorState(WorkerErrors.ValidationNameIsRequired));
        
        RuleFor(x => x.Pin)
            .InclusiveBetween(1000, 99999999)
            .WithState(x => new CustomErrorState(WorkerErrors.ValidationPinHasToHaveBetween4And8Digits));
    }
}