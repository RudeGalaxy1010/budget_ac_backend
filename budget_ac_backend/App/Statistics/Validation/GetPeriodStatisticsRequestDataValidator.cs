using budget_ac_backend.App.Operations.Requests.Data;
using FluentValidation;

namespace budget_ac_backend.App.Operations.Validation;

public class GetPeriodStatisticsRequestDataValidator : AbstractValidator<GetPeriodStatisticsRequestData> {
    public GetPeriodStatisticsRequestDataValidator() {
        RuleFor(x => x.Year)
            .NotNull()
            .WithMessage("Year cannot be null.");
    }
}
