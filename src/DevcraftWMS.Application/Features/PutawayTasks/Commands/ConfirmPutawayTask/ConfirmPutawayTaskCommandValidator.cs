using FluentValidation;

namespace DevcraftWMS.Application.Features.PutawayTasks.Commands.ConfirmPutawayTask;

public sealed class ConfirmPutawayTaskCommandValidator : AbstractValidator<ConfirmPutawayTaskCommand>
{
    public ConfirmPutawayTaskCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.LocationId).NotEmpty();
    }
}
