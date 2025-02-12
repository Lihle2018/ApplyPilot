using MediatR;

namespace JobScraper.Application.Features.Users.Commands.RegisterUser
{
    public record RegisterUserCommand(UserRequest UserRequest):IRequest;
}
