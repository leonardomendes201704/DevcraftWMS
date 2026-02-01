using FluentValidation;

namespace PerfectApiTemplate.Application.Features.Customers.Commands.DeactivateCustomer;

public sealed class DeactivateCustomerCommandValidator : AbstractValidator<DeactivateCustomerCommand>
{
    public DeactivateCustomerCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

