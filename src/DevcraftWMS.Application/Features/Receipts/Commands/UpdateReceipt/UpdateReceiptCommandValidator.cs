using FluentValidation;

namespace DevcraftWMS.Application.Features.Receipts.Commands.UpdateReceipt;

public sealed class UpdateReceiptCommandValidator : AbstractValidator<UpdateReceiptCommand>
{
    public UpdateReceiptCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.WarehouseId).NotEmpty();
        RuleFor(x => x.ReceiptNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.DocumentNumber).MaximumLength(50);
        RuleFor(x => x.SupplierName).MaximumLength(120);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}
