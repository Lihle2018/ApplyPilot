using FluentValidation;
using MediatR;
using Serilog;

namespace JobScraper.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var context = new ValidationContext<TRequest>(request);

            if (!_validators.Any())
            {
                Log.Information("No validators found for the request.");
            }
            else
            {
                Log.Information("ValidationBehavior: Found {ValidatorCount} validator(s) for {RequestName}", _validators.Count(), typeof(TRequest).Name);
                Log.Information("ValidationBehavior: Successfully handled {RequestName} - Response: {@Response}", typeof(TRequest).Name, typeof(TResponse).Name);

            }

            var failures = _validators
                .Select(v => v.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Any())
                throw new Exception(string.Join(',', failures));

            return await next();
        }
    }
}
