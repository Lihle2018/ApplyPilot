using FluentValidation;

namespace JobScraper.Application.Features.Users.Commands.RegisterUser
{
    public class RegisterUserValidator : AbstractValidator<UserRequest>
    {
        public RegisterUserValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");

            RuleFor(x => x.Surname)
                .NotEmpty().WithMessage("Surname is required.")
                .MaximumLength(50).WithMessage("Surname must not exceed 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");

            RuleFor(x => x.LinkedinLink)
                .NotEmpty().WithMessage("LinkedIn profile link is required.")
                .Must(BeAValidUrl).WithMessage("Invalid LinkedIn URL.");

            RuleFor(x => x.GitHubLink)
                .NotEmpty().WithMessage("GitHub profile link is required.")
                .Must(BeAValidUrl).WithMessage("Invalid GitHub URL.");

            RuleFor(x => x.ProfileLink)
                .NotEmpty().WithMessage("Profile link is required.")
                .Must(BeAValidUrl).WithMessage("Invalid profile URL.");

            RuleFor(x => x.Projects)
                .NotNull().WithMessage("Projects cannot be null.")
                .Must(p => p.Any()).WithMessage("At least one project is required.");

            RuleFor(x => x.Summary)
                .NotEmpty().WithMessage("Summary is required.")
                .MaximumLength(500).WithMessage("Summary must not exceed 500 characters.");
        }

        private bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}
