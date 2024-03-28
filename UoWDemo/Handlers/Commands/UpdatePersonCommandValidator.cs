using FluentValidation;

namespace UsersManagement.Handlers.Commands
{
    public class UpdatePersonCommandValidator : AbstractValidator<UpdatePersonCommand>
    {
        public UpdatePersonCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters long.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.")
                .Matches(@"^[a-zA-Zა-ჰ]+$").WithMessage("Name must contain only letters of the Georgian or Latin alphabet and must not contain both.");

            RuleFor(x => x.Surname)
                .NotEmpty().WithMessage("Surname is required.")
                .MinimumLength(2).WithMessage("Surname must be at least 2 characters long.")
                .MaximumLength(50).WithMessage("Surname must not exceed 50 characters.")
                .Matches(@"^[a-zA-Zა-ჰ]+$").WithMessage("Surname must contain only letters of the Georgian or Latin alphabet and must not contain both.");


            RuleFor(x => x.PersonalNumber)
                .NotEmpty().WithMessage("Personal number is required.")
                .Matches(@"^\d{11}$").WithMessage("Personal number must be 11 digits.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required.")
                .Must(BeAtLeast18YearsOld).WithMessage("Person must be at least 18 years old.");

            RuleFor(x => x.CityId)
                .NotEmpty().WithMessage("City identifier is required.")
                .GreaterThan(0).WithMessage("City identifier must be a positive number.");

            RuleForEach(x => x.PhoneNumbers).ChildRules(phoneNumber =>
            {
                phoneNumber.RuleFor(x => x.Number)
                    .NotEmpty().WithMessage("Phone number is required.")
                    .MinimumLength(4).WithMessage("Phone number must be at least 4 characters long.")
                    .MaximumLength(50).WithMessage("Phone number must not exceed 50 characters.");

                phoneNumber.RuleFor(x => x.NumberType)
                    .IsInEnum().WithMessage("Phone number type must be Mobile, Office, or Home.");
            });


        }

        private bool BeAtLeast18YearsOld(DateTime dateOfBirth)
        {
            return dateOfBirth <= DateTime.Today.AddYears(-18);
        }
    }
}
