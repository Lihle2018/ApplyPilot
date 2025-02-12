using JobScraper.Application.Common.Mappings;
using JobScraper.Domain.Contracts.Repositories;
using MediatR;

namespace JobScraper.Application.Features.Users.Commands.RegisterUser
{
    public class RegisterUserCommandHandler(IUserRepository repository ) : IRequestHandler<RegisterUserCommand>
    {
        public async Task Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            await repository.SaveAsync(request.UserRequest.ToUser());
        }
    }
}
