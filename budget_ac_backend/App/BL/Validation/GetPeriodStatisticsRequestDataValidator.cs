using budget_ac_backend.App.BL.Requests.Data;
using FluentValidation;

namespace budget_ac_backend.App.BL.Validation;

public class GetPeriodStatisticsRequestDataValidator : AbstractValidator<GetPeriodStatisticsRequestData> {
    public GetPeriodStatisticsRequestDataValidator() {
        RuleFor(x => x.Year)
            .NotNull()
            .WithMessage("Year cannot be null.");
    }
}
