using budget_ac_backend.App.BL.Requests.Data;
using FluentValidation;

namespace budget_ac_backend.App.BL.Validation;

public class GetCategoriesRequestDataValidator : AbstractValidator<GetCategoriesRequestData> {
    public GetCategoriesRequestDataValidator() {
        RuleFor(x => x.Offset)
            .NotNull().WithMessage("Offset cannot be null")
            .GreaterThanOrEqualTo(0).WithMessage("Offset must be greater than or equal to 0");

        RuleFor(x => x.Limit)
            .NotNull().WithMessage("Limit cannot be null")
            .GreaterThanOrEqualTo(0).WithMessage("Limit must be greater than or equal to 0")
            .NotEmpty().WithMessage("Limit cannot be empty");
    }
}