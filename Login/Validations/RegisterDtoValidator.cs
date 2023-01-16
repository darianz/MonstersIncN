using FluentValidation;
using Login.DTOs;

namespace Login.Validations
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("FirstName is required");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("LastName is required");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .Matches("^(?=.[A-Z])(?=.[0-9])(?=.*[^a-zA-Z0-9]).{8,}$").WithMessage("Password must contain at least one capital letter, one digit, and one non-alphanumeric character.");
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("PhoneNumber is required");
            RuleFor(x => x.TentaclesNumber).NotEmpty().WithMessage("TentaclesNumber is required");
        }
    }
}
