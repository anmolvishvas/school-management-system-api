using FluentValidation;
using SchoolManagement.Application.Fees.Dtos;

namespace SchoolManagement.Application.Fees.Validators;

public class CreateFeePaymentValidator : AbstractValidator<CreateFeePaymentDto>
{
    public CreateFeePaymentValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Mode).MaximumLength(50);
        RuleFor(x => x.ReferenceNo).MaximumLength(100);
    }
}
