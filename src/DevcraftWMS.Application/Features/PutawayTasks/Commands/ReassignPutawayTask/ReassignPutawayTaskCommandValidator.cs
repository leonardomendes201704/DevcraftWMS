using FluentValidation;

namespace DevcraftWMS.Application.Features.PutawayTasks.Commands.ReassignPutawayTask;

public sealed class ReassignPutawayTaskCommandValidator : AbstractValidator<ReassignPutawayTaskCommand>
{
    public ReassignPutawayTaskCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.AssigneeEmail).NotEmpty().EmailAddress();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(512);
    }
}
