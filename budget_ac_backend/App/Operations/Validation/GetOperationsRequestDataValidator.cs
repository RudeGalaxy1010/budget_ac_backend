using budget_ac_backend.App.Operations.Requests.Data;
using FluentValidation;

namespace budget_ac_backend.App.Operations.Validation;

public class GetOperationsRequestDataValidator : AbstractValidator<GetOperationsRequestData> {
    public GetOperationsRequestDataValidator() {
        RuleFor(x => x.From)
            .NotNull().WithMessage("From cannot be null.")
            .LessThanOrEqualTo(x => x.To).WithMessage("From cannot be greater than To.");

        RuleFor(x => x.To)
            .NotNull().WithMessage("To cannot be null.")
            .GreaterThanOrEqualTo(x => x.From).WithMessage("To cannot be less than From.");
    }
}