using Common.DTOs;
using FluentValidation;

namespace Business.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required.");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Valid email is required.");
            RuleFor(x => x.Password).MinimumLength(6).WithMessage("Password must be at least 6 characters.");
        }
    }
}
