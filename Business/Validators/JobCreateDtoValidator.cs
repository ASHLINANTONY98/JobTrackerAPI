using Common.DTOs;
using FluentValidation;

namespace Business.Validators
{
    public class JobCreateDtoValidator : AbstractValidator<JobCreateDto>
    {
        public JobCreateDtoValidator()
        {
            RuleFor(x => x.CompanyName).NotEmpty();
            RuleFor(x => x.PositionTitle).NotEmpty();
            RuleFor(x => x.Location).NotEmpty();
            RuleFor(x => x.AppliedDate).LessThanOrEqualTo(DateTime.UtcNow);
            RuleFor(x => x.SalaryExpectation).GreaterThanOrEqualTo(0);
        }
    }
}
