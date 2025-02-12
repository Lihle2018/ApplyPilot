
using JobScraper.Application.Features.Users.Commands.RegisterUser;
using JobScraper.Domain.Entities;

namespace JobScraper.Application.Common.Mappings
{
    public static class UserRequestMapper
    {
        public static User ToUser(this UserRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return new User(
                name: request.Name,
                surname: request.Surname,
                email: request.Email,
                phoneNumber: request.PhoneNumber,
                linkedinLink: request.LinkedinLink,
                githubLink: request.GitHubLink,
                projects: request.Projects ?? new List<string>(),
                summary: request.Summary
            );
        }
    }
}
