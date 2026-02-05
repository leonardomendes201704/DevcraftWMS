using FluentValidation;

namespace DevcraftWMS.Application.Features.PickingTasks.Commands.ConfirmPickingTask;

public sealed class ConfirmPickingTaskCommandValidator : AbstractValidator<ConfirmPickingTaskCommand>
{
    public ConfirmPickingTaskCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.PickingTaskItemId).NotEmpty();
            item.RuleFor(i => i.QuantityPicked).GreaterThanOrEqualTo(0);
        });
    }
}
