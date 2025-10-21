using FluentValidation;
using Common.DTOs;

namespace Business.Validators
{
    public class JobUpdateDtoValidator : AbstractValidator<JobUpdateDto>
    {
        public JobUpdateDtoValidator()
        {
            When(x => x.CompanyName != null, () =>
                RuleFor(x => x.CompanyName).NotEmpty().WithMessage("Company name cannot be empty."));

            When(x => x.PositionTitle != null, () =>
                RuleFor(x => x.PositionTitle).NotEmpty().WithMessage("Position title cannot be empty."));

            When(x => x.Location != null, () =>
                RuleFor(x => x.Location).NotEmpty().WithMessage("Location cannot be empty."));

            When(x => x.AppliedDate.HasValue, () =>
                RuleFor(x => x.AppliedDate.Value).LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Applied date cannot be in the future."));

            When(x => x.SalaryExpectation.HasValue, () =>
                RuleFor(x => x.SalaryExpectation.Value).GreaterThanOrEqualTo(0).WithMessage("Salary must be zero or positive."));
        }
    }
}
