using FluentValidation;
using SchoolManagement.Application.Fees.Dtos;

namespace SchoolManagement.Application.Fees.Validators;

public class CreateFeeInvoiceValidator : AbstractValidator<CreateFeeInvoiceDto>
{
    public CreateFeeInvoiceValidator()
    {
        RuleFor(x => x.StudentId).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Discount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}
